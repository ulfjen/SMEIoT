using System.Collections.Generic;
using NodaTime;
using SMEIoT.Core.Entities;
using Newtonsoft.Json;

namespace SMEIoT.Web.ApiModels
{
  public class AdminUserApiModel
  {
    [JsonProperty(Required = Required.DisallowNull)]
    public long Id { get; set; }

    [JsonProperty(Required = Required.DisallowNull)]
    public string UserName { get; set; }

    [JsonProperty(Required = Required.DisallowNull)]
    public IEnumerable<string> Roles { get; set; }

    [JsonProperty(Required = Required.DisallowNull)]
    public Instant CreatedAt { get; set; }

    [JsonProperty(Required = Required.DisallowNull)]
    public Instant LastSeenAt { get; set; }

    public AdminUserApiModel(User user, IList<string> roles)
    {
      Id = user.Id;
      UserName = user.UserName;
      Roles = roles;
      CreatedAt = user.CreatedAt;
      LastSeenAt = user.LastSeenAt;
    }
  }
}
