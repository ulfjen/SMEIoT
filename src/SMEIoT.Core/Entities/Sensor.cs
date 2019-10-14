using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NodaTime;

namespace SMEIoT.Core.Entities
{
  public class Sensor : IAuditTimestamp
  {
    [Key]
    public long Id { get; set; }
    
    public string Name { get; set; }
    public string NormalizedName { get; set; }
    
    public List<UserSensor> UserSensors { get; set; }
    
    public Instant CreatedAt { get; set; }
    public Instant UpdatedAt { get; set; }

    public static string NormalizeName(string name)
    {
      return name.ToUpperInvariant();
    }
  }
}
