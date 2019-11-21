using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SMEIoT.Web.BindingModels
{
  public class DeviceConfigBindingModel 
  {
    [Required]
    [JsonProperty(Required = Required.DisallowNull)]
    public string Name { get; set; }
    
    [Required]
    [JsonProperty(Required = Required.DisallowNull)]
    public string Identity { get; set; }
    
    [Required]
    [JsonProperty(Required = Required.DisallowNull)]
    public string Key { get; set; }
  }
}
