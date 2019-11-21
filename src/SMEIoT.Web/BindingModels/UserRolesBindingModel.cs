using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SMEIoT.Web.BindingModels
{
  public class UserRolesBindingModel
  {
    [Required]
    [JsonProperty(Required = Required.DisallowNull)]
    public IEnumerable<string> Roles { get; set; }
  }
}
