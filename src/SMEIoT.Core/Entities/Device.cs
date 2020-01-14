using System.Collections.Generic;

namespace SMEIoT.Core.Entities
{
  public class Device : MqttEntityBase
  {
    public DeviceAuthenticationType AuthenticationType { get; set; }
    public string? PreSharedKey { get; set; }

    public List<Sensor> Sensors { get; set; } = new List<Sensor>();
  }
}
