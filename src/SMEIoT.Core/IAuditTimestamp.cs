using NodaTime;

namespace SMEIoT.Core
{
  public interface IAuditTimestamp
  {
    Instant CreatedAt { get; set; }
    Instant UpdatedAt { get; set; }
  }
}
