using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace SMEIoT.Web.BindingModels
{
  public class DeviceConfigBindingModel 
  {
    [BindRequired]
    [JsonProperty(Required = Required.DisallowNull)]
    public string Name { get; set; } = null!;
    
    [BindRequired]
    [JsonProperty(Required = Required.DisallowNull)]
    public string Identity { get; set; } = null!;
    
    [BindRequired]
    [JsonProperty(Required = Required.DisallowNull)]
    public string Key { get; set; } = null!;
  }
}
