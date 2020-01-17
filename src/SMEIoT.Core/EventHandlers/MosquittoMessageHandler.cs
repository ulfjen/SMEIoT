using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using NodaTime;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace SMEIoT.Core.EventHandlers
{
  public class MosquittoMessageHandler : IMosquittoMessageHandler
  {
    private readonly IMqttMessageDispatchService _service;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger _logger;
    private readonly IClock _clock;

    public MosquittoMessageHandler(
        IClock clock,
        IMqttMessageDispatchService service, 
        ILogger<MosquittoMessageHandler> logger)
    {
      _clock = clock;
      _service = service;
      _logger = logger;
    }

    public void HandleMessage(int mid, string topic, IntPtr payload, int payloadlen, int qos, int retain)
    {
      var copied = new byte[payloadlen];
      Marshal.Copy(payload, copied, 0, payloadlen);
      var decoded = Encoding.UTF8.GetString(copied, 0, copied.Length);
      _logger.LogTrace($"Received {decoded}");

      var message = new MqttMessage(topic, decoded, _clock.GetCurrentInstant());

      // do something to send messages
      // 1. maintain internal logs, and maybe store it
      // 2. log to the user on the web
      ThreadPool.QueueUserWorkItem(async (o) => {
        await _service.ProcessAsync(message);
      });
    }
  }
}
