using System.Collections.Generic;
using SMEIoT.Core.Entities;

namespace SMEIoT.Web.ApiModels
{
  public class UserCredentialsUpdateApiModel : BasicUserApiModel
  {
    public bool PasswordUpdated { get; set; }

    public UserCredentialsUpdateApiModel(User user, IList<string> roles) : base(user, roles)
    {
    }
  }
}
