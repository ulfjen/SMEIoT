using NodaTime;

namespace SMEIoT.Core.Interfaces
{
  public interface IMqttConnectionStamp
  {
    /// Cached connection status for last 10 minutes
    bool Connected { get; set; }
    Instant? ConnectedAt { get; set; }
    Instant? LastMessageAt { get; set; }
  }
}
