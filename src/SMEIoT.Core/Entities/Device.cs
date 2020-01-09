using System;
using System.Collections.Generic;
using SMEIoT.Core.Interfaces;

namespace SMEIoT.Core.Entities
{
  public class Device : MqttEntityBase
  {
    public DeviceAuthenticationType AuthenticationType { get; set; }
    public string? PreSharedKey { get; set; }

    public List<Sensor> Sensors { get; set; } = new List<Sensor>();
  }
}
