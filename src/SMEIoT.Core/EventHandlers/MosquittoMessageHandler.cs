using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using NodaTime;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Interfaces;

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

    public MosquittoMessageHandler(IClock clock, IMqttIdentifierService mqttService, IMosquittoBrokerService brokerService)
    {
      _clock = clock;
      _mqttService = mqttService;
      _brokerService = brokerService;
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

    public void Update(MqttMessage message)
    {
      var topic = message.Topic;
      Console.WriteLine(message.Payload);

      if (topic.StartsWith(SensorTopicPrefix))
      {
        var parsed = topic.AsSpan();
        parsed = parsed.Slice(SensorTopicPrefix.Length);
        var splitP = parsed.IndexOf('/');
        if (splitP != -1) {
          var deviceName = parsed.Slice(0, splitP).ToString();
          if (!_mqttService.RegisterDeviceName(deviceName))
          {
            // TODO: report device registeration failed
          } else
          {
            var sensorName = parsed.Slice(splitP).ToString();
             if (!_mqttService.RegisterSensorNameWithDeviceName(sensorName, deviceName))
            {
              // TODO: report sensor registeration failed
            }
          }
        } else
        {
          // TODO: generates parsed error
        }
      } else if (topic.StartsWith(BrokerTopicPrefix)) {
        var parsed = topic.AsSpan();
        parsed = parsed.Slice(BrokerTopicPrefix.Length);
        var value = message.Payload.AsSpan();

        if (value.EndsWith(SecondsPostfix)) {
          value = value.Slice(0, value.Length - SecondsPostfix.Length).TrimEnd();
        }
        _brokerService.RegisterBrokerStatistics(parsed.ToString(), value.ToString());

        _brokerService.BrokerRunning = true;
        _brokerService.BrokerLastUpdatedAt = message.ReceivedAt;
      }

      // TODO: Sends a job into dispatch for storage
    }
  }
}
