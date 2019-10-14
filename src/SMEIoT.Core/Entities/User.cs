using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using NodaTime;

namespace SMEIoT.Core.Entities
{
  public class User : IdentityUser<long>, IAuditTimestamp
  {
    public List<UserSensor> UserSensors { get; set; }
    
    public Instant CreatedAt { get; set; }
    public Instant UpdatedAt { get; set; }
    public Instant LastSeenAt { get; set; }
  }
}
