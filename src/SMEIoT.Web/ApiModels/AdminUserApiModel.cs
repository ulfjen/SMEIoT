using System.Collections.Generic;
using NodaTime;
using System;
using SMEIoT.Core.Entities;
using Newtonsoft.Json;

namespace SMEIoT.Web.ApiModels
{
  public class AdminUserApiModel
  {
    public long Id { get; set; }

    [JsonProperty(Required = Required.DisallowNull)]
    public string UserName { get; set; }

    [JsonProperty(Required = Required.DisallowNull)]
    public IEnumerable<string> Roles { get; set; }

    public Instant CreatedAt { get; set; }

    public Instant LastSeenAt { get; set; }

    public AdminUserApiModel(User user, IEnumerable<string> roles)
    {
      Id = user.Id;
      UserName = user.UserName;
      Roles = roles;
      CreatedAt = user.CreatedAt;
      LastSeenAt = user.LastSeenAt;
    }
  }
}
