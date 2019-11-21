using System.Collections.Generic;
using Newtonsoft.Json;

namespace SMEIoT.Web.ApiModels
{
  public class AdminUserApiModelList
  {
    [JsonProperty(Required = Required.DisallowNull)]
    public IEnumerable<AdminUserApiModel> Users { get; set; }

    public AdminUserApiModelList(IEnumerable<AdminUserApiModel> users)
    {
      Users = users;
    }
  }
}
