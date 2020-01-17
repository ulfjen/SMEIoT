using System;
using System.ComponentModel.DataAnnotations;
using NodaTime;

namespace SMEIoT.Core.Entities
{
  public class SensorValue
  {
    [Required]
    public long SensorId { get; set; }
    private Sensor? _sensor = null;
    public Sensor Sensor
    {
      get => _sensor ?? throw new InvalidOperationException("Uninitialized property: " + nameof(Sensor));
      set => _sensor = value;
    }
    public double Value { get; set; }
    public Instant CreatedAt { get; set; }
  }
}
