using System.ComponentModel.DataAnnotations;
using NodaTime;

namespace SMEIoT.Core.Entities
{
  public class SensorValue
  {
    [Key]
    public long Id { get; set; }

    [Required]
    public long SensorId { get; set; }
    public Sensor Sensor { get; set; } = null!;
    public double Value { get; set; }
    public Instant CreatedAt { get; set; }
  }
}
