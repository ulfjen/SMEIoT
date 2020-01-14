using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMEIoT.Core.Entities
{
  public class Sensor : MqttEntityBase
  {
    [Required]
    public long DeviceId { get; set; }
    private Device? _device = null;
    public Device Device
    {
      get => _device ?? throw new InvalidOperationException("Uninitialized property: " + nameof(Device));
      set => _device = value;
    }
    public List<UserSensor> UserSensors { get; set; } = new List<UserSensor>();

    public List<SensorValue> SensorValues { get; set; } = new List<SensorValue>();
  }
}
