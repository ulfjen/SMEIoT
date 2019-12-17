using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SMEIoT.Infrastructure.MqttClient
{
  public class BackgroundMqttClientHostedService : IHostedService, IDisposable
  {
    private readonly MosquittoClient _client;
    private Timer? _timer;
    private ILogger<BackgroundMqttClientHostedService> _logger;
    private readonly int _delay = 100;
    private bool _reconnect;
    private bool _stoppedTimer;

    public BackgroundMqttClientHostedService(MosquittoClient client, ILogger<BackgroundMqttClientHostedService> logger)
    {
      _client = client;
      _logger = logger;
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

      _client.Connect();
      _reconnect = false;
      _stoppedTimer = false;
      _timer = new Timer(ExecuteRunLoop, null, 0, _delay);
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
