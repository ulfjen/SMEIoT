using NodaTime;

namespace SMEIoT.Core.Interfaces
{
  public interface IAuditTimestamp
  {
    Instant CreatedAt { get; set; }
    Instant UpdatedAt { get; set; }
  }
}
