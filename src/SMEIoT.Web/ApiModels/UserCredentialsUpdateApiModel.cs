using System.Collections.Generic;
using SMEIoT.Core.Entities;
using Newtonsoft.Json;

namespace SMEIoT.Web.ApiModels
{
  public class UserCredentialsUpdateApiModel : BasicUserApiModel
  {
    [JsonProperty(Required = Required.DisallowNull)]
    public bool PasswordUpdated { get; set; }

    public UserCredentialsUpdateApiModel(User user, IList<string> roles) : base(user, roles)
    {
    }
  }
}
