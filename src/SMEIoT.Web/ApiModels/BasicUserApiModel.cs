using System.Collections.Generic;
using NodaTime;
using SMEIoT.Core.Entities;

namespace SMEIoT.Web.ApiModels
{
  public class BasicUserApiModel
  {
    public string Username { get; set; }
    public IEnumerable<string> Roles { get; set; }
    public Instant CreatedAt { get; set; }

    public BasicUserApiModel(User user, IList<string> roles)
    {
      Username = user.UserName;
      Roles = roles;
      CreatedAt = user.CreatedAt;
    }
  }
}
