using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SMEIoT.Web.BindingModels
{
  public class DeviceBootstrapConfigBindingModel
  {
    [Required]
    [JsonProperty(Required = Required.DisallowNull)]
    public string Identity { get; set; }
    
    [Required]
    [JsonProperty(Required = Required.DisallowNull)]
    public string Key { get; set; }
  }
}
