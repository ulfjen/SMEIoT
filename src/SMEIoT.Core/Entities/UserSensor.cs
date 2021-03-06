using System;
using System.ComponentModel.DataAnnotations;

namespace SMEIoT.Core.Entities
{
  public class UserSensor
  {
    [Required]
    public long UserId { get; set; }
    private User? _user = null;
    public User User
    {
      get => _user ?? throw new InvalidOperationException("Uninitialized property: " + nameof(User));
      set => _user = value;
    }

    [Required]
    public long SensorId { get; set; }
    private Sensor? _sensor = null;
    public Sensor Sensor
    {
      get => _sensor ?? throw new InvalidOperationException("Uninitialized property: " + nameof(Sensor));
      set => _sensor = value;
    }
  }
}
