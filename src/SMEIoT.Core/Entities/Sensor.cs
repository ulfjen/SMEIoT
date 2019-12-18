using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NodaTime;

namespace SMEIoT.Core.Entities
{
  public class Sensor : IAuditTimestamp
  {
    [Key]
    public long Id { get; set; }
    
    public string Name { get; set; } = null!;
    public string NormalizedName { get; set; } = null!;
    
    [Required]
    public long DeviceId { get; set; }
    public Device Device { get; set; } = null!;
    public List<UserSensor> UserSensors { get; set; } = null!;

    public List<SensorValue> SensorValues { get; set; } = null!;
    
    public Instant CreatedAt { get; set; }
    public Instant UpdatedAt { get; set; }

    public static string NormalizeName(string name)
    {
      return name.ToUpperInvariant();
    }
  }
}
