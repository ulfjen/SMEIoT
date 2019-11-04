using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace SMEIoT.Infrastructure.MqttClient
{
  public class BackgroundMqttClientHostedService : IHostedService, IDisposable
  {
    private readonly MosquittoClient _client;
    private Thread? _thread;

    public BackgroundMqttClientHostedService(
        MosquittoClient client)
    {
      _client = client;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
      _client.Connect();
      _thread = new Thread(() => _client.RunLoop());
      _thread.Start();
      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      if (_thread != null)
      {
        _thread.Abort();
      }
      return Task.CompletedTask;
    }

    public void Dispose()
    {
      if (_thread != null)
      {
        _thread.Abort();
        _thread = null;
      }
      _client.Dispose();
    }

  }
}
