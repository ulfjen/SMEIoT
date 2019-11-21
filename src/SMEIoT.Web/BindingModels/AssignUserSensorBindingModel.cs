using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SMEIoT.Web.BindingModels
{
  public class AssignUserSensorBindingModel
  {
    [Required]
    [JsonProperty(Required = Required.DisallowNull)]
    public string Username { get; set; }

  }
}
