using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace SMEIoT.Web.BindingModels
{
  public class DeviceBootstrapConfigBindingModel
  {
    [BindRequired]
    [JsonProperty(Required = Required.DisallowNull)]
    public string Identity { get; set; } = null!;
    
    [BindRequired]
    [JsonProperty(Required = Required.DisallowNull)]
    public string Key { get; set; } = null!;
  }
}
