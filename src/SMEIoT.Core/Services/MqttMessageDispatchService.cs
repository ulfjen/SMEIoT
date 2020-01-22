using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using SMEIoT.Core.Interfaces;
using SMEIoT.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SMEIoT.Core.Services
{
  public class MqttMessageDispatchService : IMqttMessageDispatchService
  {
    private readonly IServiceScopeFactory _scopeFactory;

    public MqttMessageDispatchService(IServiceScopeFactory scopeFactory)
    {
      _scopeFactory = scopeFactory;
    }

    public async Task ProcessAsync(MqttMessage message)
    {
      using var scope = _scopeFactory.CreateScope();
      var ingest = scope.ServiceProvider.GetRequiredService<IMqttMessageIngestionService>();
      var relay = scope.ServiceProvider.GetRequiredService<IMqttMessageRelayService>();
      var logger = scope.ServiceProvider.GetService<ILogger<MqttMessageDispatchService>>();
      
      try {
        await ingest.ProcessCommonMessageAsync(message);
        await ingest.ProcessBrokerMessageAsync(message);
        await relay.RelayAsync(message);
      } catch (Exception exception) {
        if (logger != null) {
          logger.LogWarning($"Exception was thrown when processes mqtt message {message.Topic}. {exception.Message}");
        }

      }
    }
  }
}
