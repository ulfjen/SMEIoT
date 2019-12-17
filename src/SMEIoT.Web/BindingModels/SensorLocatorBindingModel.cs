using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace SMEIoT.Web.BindingModels
{
  public class SensorLocatorBindingModel
  {
    [BindRequired]
    [JsonProperty(Required = Required.DisallowNull)]
    public string DeviceName { get; set; } = null!;

    [BindRequired]
    [JsonProperty(Required = Required.DisallowNull)]
    public string Name { get; set; } = null!;
  }
}
