using NodaTime;

namespace SMEIoT.Core.Entities
{
  public class MqttMessage
  {
    public string Topic { get; set; }
    public string Payload { get; set; }
    public Instant ReceivedAt { get; set; }

    public MqttMessage(string topic, string payload, Instant receivedAt)
    {
      Topic = topic;
      Payload = payload;
      ReceivedAt = receivedAt;
    }
  }
}
