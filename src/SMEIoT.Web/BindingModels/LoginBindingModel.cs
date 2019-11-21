using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace SMEIoT.Web.BindingModels
{
  public class LoginBindingModel
  {
    [BindRequired]
    [JsonProperty(Required = Required.DisallowNull)]
    public string Username { get; set; } = null!;

    [BindRequired]
    [JsonProperty(Required = Required.DisallowNull)]
    public string Password { get; set; } = null!;
    
    [Url]
    [JsonProperty(Required = Required.DisallowNull)]
    public string? ReturnUrl { get; set; }
  }
}
