using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using SMEIoT.Core.Interfaces;

namespace SMEIoT.Infrastructure.MqttClient
{
  public class BackgroundMqttClientHostedService : IHostedService, IDisposable
  {
    private readonly IMosquittoClientService _client;
    private Timer? _timer;
    private ILogger _logger;
    private readonly IMosquittoBrokerService _brokerService;
    private readonly IHostEnvironment _env;
    private readonly int _delay = 100;
    private bool _reconnect;
    private bool _stoppedTimer;

    public BackgroundMqttClientHostedService(
      IMosquittoClientService client,
      ILogger<BackgroundMqttClientHostedService> logger,
      IMosquittoBrokerService brokerService,
      IHostEnvironment env)
    {
      _client = client;
      _logger = logger;
      _brokerService = brokerService;
      _env = env;
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

    private void ConnectClient(object state)
    {
      _logger.LogInformation("Start to connect MQTT client with the Mosquitto broker.");

      RestartBrokerIfBrokerWasRogue(false);
      _client.Connect();

      _reconnect = false;
      _stoppedTimer = false;
      _timer = new Timer(ExecuteRunLoop, null, 0, _delay);
    }

    private void RestartBrokerIfBrokerWasRogue(bool ignoreAuthPluginPid)
    {
      // broker runs but not the same
      _logger.LogDebug($"Broker pid = {_brokerService.BrokerPid}; auth plugin reports = {_brokerService.BrokerPidFromAuthPlugin} ");
      if (_brokerService.BrokerPid != null && (_brokerService.BrokerPidFromAuthPlugin == null || _brokerService.BrokerPid.Value != _brokerService.BrokerPidFromAuthPlugin.Value)) { 
        _brokerService.RestartBrokerBySignalAsync(true).GetAwaiter().GetResult();
        int waitingTurn = 5;
        while (waitingTurn-- > 0) {
          if (_brokerService.BrokerPidFromAuthPlugin == null) {
            Thread.Sleep(TimeSpan.FromSeconds(5)); // waiting the broker to be set up. 
          } else {
            break;
          }
        }
        _logger.LogDebug($"Broker pid = {_brokerService.BrokerPid}; auth plugin reports = {_brokerService.BrokerPidFromAuthPlugin} ");
        if (_brokerService.BrokerPidFromAuthPlugin == null && !_env.IsProduction()) {
          throw new OperationCanceledException("Broker is not responding with our restart.");
        }
      }
    }

    private void ExecuteRunLoop(object state)
    {
      try
      {
        if (_reconnect) {
          var ret = _client.Reconnect();
          _logger.LogInformation($"MQTT client failed, trying to reconnect. got status: {ret}");
  
          if (ret == 0) {
            _reconnect = false;
          }
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
        if (!_stoppedTimer) {
          _timer.Change(_delay, Timeout.Infinite);
        } else {
          _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }
      }
    }
    
    public Task StopAsync(CancellationToken cancellationToken)
    {
      // timer may still run a few times but that's fine
      _stoppedTimer = true;
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
