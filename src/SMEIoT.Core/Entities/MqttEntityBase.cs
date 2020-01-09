using System.ComponentModel.DataAnnotations;
using NodaTime;
using SMEIoT.Core.Interfaces;

namespace SMEIoT.Core.Entities
{
  public abstract class MqttEntityBase : IAuditTimestamp, IMqttConnectionStamp
  {
    [Key]
    public long Id { get; set; }
    
    public string Name { get; set; } = null!;
    public string NormalizedName { get; set; } = null!;
    
    public Instant CreatedAt { get; set; }
    public Instant UpdatedAt { get; set; }

    public bool Connected { get; set; }
    public Instant? ConnectedAt { get; set; }
    public Instant? LastMessageAt { get; set; }

    public static string NormalizeName(string name)
    {
      return name.ToUpperInvariant();
    }
  }
}
