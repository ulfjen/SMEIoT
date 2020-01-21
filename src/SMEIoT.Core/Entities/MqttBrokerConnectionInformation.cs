namespace SMEIoT.Core.Entities
{
  public class MqttBrokerConnectionInformation
  {
    public string Host { get; set; } = null!;
    public int Port { get; set; }
    public string TopicPrefix = null!;
  }
}
