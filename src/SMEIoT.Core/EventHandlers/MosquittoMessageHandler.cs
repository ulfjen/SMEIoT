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
    private readonly List<IMqttMessageObserver> _observers = new List<IMqttMessageObserver>();
    private readonly IClock _clock;
    private readonly IMqttSensorService _mqttService;

    public MosquittoMessageHandler(IClock clock, IMqttSensorService mqttService)
    {
      _clock = clock;
      _mqttService = mqttService;
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
      if (topic.StartsWith("sensor/"))
      {
        _mqttService.RegisterSensorByName(topic.Substring(7));
      }
      // TODO: Sends a job into dispatch for storage
    }
  }
}
