using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SMEIoT.Web.BindingModels
{
  public class SensorLocatorBindingModel
  {
    [Required]
    [JsonProperty(Required = Required.DisallowNull)]
    public string Name { get; set; }
  }
}
