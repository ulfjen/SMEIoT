using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace SMEIoT.Web.BindingModels
{
  public class AssignUserSensorBindingModel
  {
    [BindRequired]
    [JsonProperty(Required = Required.DisallowNull)]
    public string UserName { get; set; } = null!;
  }
}
