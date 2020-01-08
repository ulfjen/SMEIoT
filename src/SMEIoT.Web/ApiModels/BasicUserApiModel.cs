using System.Collections.Generic;
using NodaTime;
using SMEIoT.Core.Entities;
using Newtonsoft.Json;

namespace SMEIoT.Web.ApiModels
{
  public class BasicUserApiModel
  {
    [JsonProperty(Required = Required.DisallowNull)]
    public string UserName { get; set; }
    
    [JsonProperty(Required = Required.DisallowNull)]
    public IEnumerable<string> Roles { get; set; }
    
    public Instant CreatedAt { get; set; }

    public BasicUserApiModel(User user, IList<string> roles)
    {
      UserName = user.UserName;
      Roles = roles;
      CreatedAt = user.CreatedAt;
    }
  }
}
