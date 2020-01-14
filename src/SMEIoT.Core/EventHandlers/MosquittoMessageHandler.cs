using System;
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
  public class MosquittoMessageHandler : IMqttMessageObserver
  {
    public const string SensorTopicPrefix = "iot/";
    public const string BrokerTopicPrefix = "$SYS/broker/";
    private const string SecondsPostfix = "seconds";

    private readonly List<IMqttMessageObserver> _observers = new List<IMqttMessageObserver>();
    private readonly IClock _clock;
    private readonly IMqttIdentifierService _mqttService;
    private readonly IMosquittoBrokerService _brokerService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger _logger;

    public MosquittoMessageHandler(
        IClock clock,
        IMqttIdentifierService mqttService, 
        IMosquittoBrokerService brokerService,
        IServiceScopeFactory scopeFactory,
        ILogger<MosquittoMessageHandler> logger)
    {
      _clock = clock;
      _mqttService = mqttService;
      _brokerService = brokerService;
      _scopeFactory = scopeFactory;
      _logger = logger;
      Attach(this);
    }

    public void Attach(IMqttMessageObserver observer)
    {
      _observers.Add(observer);
    }

    public void Detach(IMqttMessageObserver observer)
    {
      _observers.Remove(observer);
    }

    public void Notify(MqttMessage message)
    {
      foreach (var o in _observers)
      {
        o.Update(message);
      }
    }

    public void HandleMessage(int mid, string topic, IntPtr payload, int payloadlen, int qos, int retain)
    {
      var copied = new byte[payloadlen];
      Marshal.Copy(payload, copied, 0, payloadlen);
      var decoded = Encoding.UTF8.GetString(copied, 0, copied.Length);

      var message = new MqttMessage(topic, decoded, _clock.GetCurrentInstant());
      Notify(message);
    }

    private void ProcessSensorTopic(MqttMessage message)
    {
      var parsed = message.Topic.AsSpan();
      parsed = parsed.Slice(SensorTopicPrefix.Length);
      var splitP = parsed.IndexOf('/');
      if (splitP != -1) {
        _logger.LogTrace($"{parsed.Slice(0, splitP).ToString()}:{parsed.Slice(splitP+1).ToString()}");
      } else {
        _logger.LogTrace(parsed.ToString());
      }

      if (splitP != -1) {
        var deviceName = parsed.Slice(0, splitP).ToString();
        _mqttService.RegisterDeviceNameAsync(deviceName).GetAwaiter().GetResult();
          // TODO: report device registeration failed
        var sensorName = parsed.Slice(splitP+1).ToString();
        _mqttService.RegisterSensorNameWithDeviceNameAsync(sensorName, deviceName).GetAwaiter().GetResult();
          // TODO: report sensor registeration failed
        using (var scope = _scopeFactory.CreateScope())
        {
          var service = scope.ServiceProvider.GetService<IDeviceService>();
          var valueService = scope.ServiceProvider.GetService<ISensorValueService>();

          // try {
            var device = service.GetDeviceByNameAsync(deviceName).GetAwaiter().GetResult();
            var sensor = service.GetSensorByDeviceAndNameAsync(device, sensorName).GetAwaiter().GetResult();
            _logger.LogTrace($"{sensor.Name} << {double.Parse(message.Payload).ToString()} at {message.ReceivedAt}");
            valueService.AddSensorValueAsync(sensor, double.Parse(message.Payload), message.ReceivedAt).GetAwaiter().GetResult();
            _logger.LogTrace(message.Payload);
          // } catch {
            // catch saving errors or finding
          // }
        }
      } else
      {
        // TODO: generates parsed error
      }
      // TODO: Sends a job into dispatch for storage
    }

    private void ProcessBrokerTopic(MqttMessage message)
    {
      var parsed = message.Topic.AsSpan();
      parsed = parsed.Slice(BrokerTopicPrefix.Length);
      var value = message.Payload.AsSpan();

      if (value.EndsWith(SecondsPostfix)) {
        value = value.Slice(0, value.Length - SecondsPostfix.Length).TrimEnd();
      }
      _ = _brokerService.RegisterBrokerStatisticsAsync(parsed.ToString(), value.ToString(), message.ReceivedAt).GetAwaiter().GetResult();

      _brokerService.BrokerLastMessageAt = message.ReceivedAt;
    }

    public void Update(MqttMessage message)
    {
      _logger.LogTrace($"{message.Topic} = {message.Payload}");

      if (message.Topic.StartsWith(SensorTopicPrefix))
      {
        ProcessSensorTopic(message);
      }
      else if (message.Topic.StartsWith(BrokerTopicPrefix))
      {
        ProcessBrokerTopic(message);
      }
    }
  }
}
