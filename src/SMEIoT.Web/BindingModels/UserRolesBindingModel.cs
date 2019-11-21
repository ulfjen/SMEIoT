using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace SMEIoT.Web.BindingModels
{
  public class UserRolesBindingModel
  {
    [BindRequired]
    [JsonProperty(Required = Required.DisallowNull)]
    public IEnumerable<string> Roles { get; set; } = null!;
  }
}
