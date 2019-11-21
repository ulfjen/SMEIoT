using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SMEIoT.Web.BindingModels
{
  public class LoginBindingModel
  {
    [Required]
    [JsonProperty(Required = Required.DisallowNull)]
    public string Username { get; set; }

    [Required]
    [JsonProperty(Required = Required.DisallowNull)]
    public string Password { get; set; }
    
    [Url]
    [JsonProperty(Required = Required.DisallowNull)]
    public string? ReturnUrl { get; set; }
  }
}
