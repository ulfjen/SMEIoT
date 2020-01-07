using System.Collections.Generic;
using Newtonsoft.Json;

namespace SMEIoT.Web.ApiModels
{
  public class AdminUserApiModelList
  {
    [JsonProperty(Required = Required.DisallowNull)]
    public IEnumerable<AdminUserApiModel> Users { get; set; }

    // the total amount of items that the query can return.
    [JsonProperty(Required = Required.DisallowNull)]
    public int Total { get; set; }

    public AdminUserApiModelList(IEnumerable<AdminUserApiModel> users, int total)
    {
      Users = users;
      Total = total;
    }
  }
}
