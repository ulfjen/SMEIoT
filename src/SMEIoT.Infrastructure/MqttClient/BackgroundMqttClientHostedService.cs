using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SMEIoT.Core.Interfaces;
using Microsoft.Extensions.Configuration;


namespace SMEIoT.Infrastructure.MqttClient
{
  public class BackgroundMqttClientHostedService : IHostedService, IDisposable
  {
    private readonly IMosquittoClientService _client;
    private Timer? _timer;
    private ILogger _logger;
    private readonly IMosquittoBrokerService _service;
    private readonly IMosquittoBrokerPidAccessor _accessor;
    private readonly IMosquittoBrokerPluginPidService _pluginService;
    private readonly IHostEnvironment _env;
    private int _delay;
    private bool _reconnect;

    public BackgroundMqttClientHostedService(
      IMosquittoClientService client,
      ILogger<BackgroundMqttClientHostedService> logger,
      IMosquittoBrokerService service,
      IMosquittoBrokerPidAccessor accessor,
      IMosquittoBrokerPluginPidService pluginService,
      IHostEnvironment env,
      IConfiguration config)
    {
      _client = client;
      _logger = logger;
      _service = service;
      _accessor = accessor;
      _pluginService = pluginService;
      _env = env;

      var interval = config.GetSection("SMEIoT")?.GetValue<int>("MosquittoBackgroundClientRunloopInteval");
      if (interval.HasValue && interval.Value > 0) {
         _delay = interval.Value;
      } else {
        throw new InvalidOperationException("SMEIoT__MosquittoBackgroundClientRunloopInteval must be set to a positive millis.");
      }
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
      try
      {
        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        ThreadPool.QueueUserWorkItem(ConnectClient);
      }
      catch (OperationCanceledException)
      {
      }
    }

    private void ConnectClient(object? state)
    {
      _logger.LogInformation("Start to connect MQTT client with the Mosquitto broker.");

      RestartBrokerIfBrokerWasRogue(false);
      _client.Connect();

      _reconnect = false;
      _timer = new Timer(ExecuteRunLoop, null, 0, _delay);
    }

    private void RestartBrokerIfBrokerWasRogue(bool ignoreAuthPluginPid)
    {
      // broker runs but not the same
      _logger.LogDebug($"Broker pid = {_accessor.BrokerPid}; auth plugin reports = {_pluginService.BrokerPidFromAuthPlugin} ");
      if (_accessor.BrokerPid != null && (_pluginService.BrokerPidFromAuthPlugin == null || _accessor.BrokerPid.Value != _pluginService.BrokerPidFromAuthPlugin.Value)) { 
        _service.RestartBrokerBySignalAsync(true).GetAwaiter().GetResult();
        int waitingTurn = 5;
        while (waitingTurn-- > 0) {
          if (_pluginService.BrokerPidFromAuthPlugin == null) {
            Thread.Sleep(TimeSpan.FromSeconds(5)); // waiting the broker to be set up. 
          } else {
            break;
          }
        }
        _logger.LogDebug($"Broker pid = {_accessor.BrokerPid}; auth plugin reports = {_pluginService.BrokerPidFromAuthPlugin} ");
        if (_pluginService.BrokerPidFromAuthPlugin == null && !_env.IsProduction()) {
          throw new OperationCanceledException("Broker is not responding with our restart.");
        }
      }
    }

    private void ExecuteRunLoop(object? state)
    {
      try
      {
        if (_reconnect) {
          _client.Connect();
          _logger.LogInformation($"MQTT client failed, trying to reconnect.");
  
          _reconnect = false;
        } else {
          var ret = _client.RunLoop();
          _logger.LogTrace($"runloop returned: {ret}");

          if (ret == 5) { // connection refused.
            throw new SystemException("We can't connect with the broker. Socket error.");
          }
          if (ret != 0) { // try to reconnect next time
            _reconnect = true;
          }
        } 
      }
      finally
      {
        if (_timer != null) {
          _timer.Change(_delay, Timeout.Infinite);
        }
      }
    }
    
    public Task StopAsync(CancellationToken cancellationToken)
    {
      // timer may still run a few times but that's fine
      _timer?.Change(Timeout.Infinite, Timeout.Infinite);

      return Task.CompletedTask;
    }

    public void Dispose()
    {       
      _timer?.Dispose();
      _client.Dispose();
    }

  }
}
