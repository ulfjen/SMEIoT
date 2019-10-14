using System.Collections.Generic;
using NodaTime;
using SMEIoT.Core.Entities;

namespace SMEIoT.Web.ApiModels
{
  public class AdminUserApiModel
  {
    public long Id { get; set; }
    public string Username { get; set; }
    public IEnumerable<string> Roles { get; set; }
    public Instant CreatedAt { get; set; }
    public Instant LastSeenAt { get; set; }

    public AdminUserApiModel(User user, IList<string> roles)
    {
      Id = user.Id;
      Username = user.UserName;
      Roles = roles;
      CreatedAt = user.CreatedAt;
      LastSeenAt = user.LastSeenAt;
    }
  }
}
