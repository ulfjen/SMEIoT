using Newtonsoft.Json;

namespace SMEIoT.Web.ApiModels
{
  public class LoginedApiModel
  {
    [JsonProperty(Required = Required.DisallowNull)]
    public string ReturnUrl { get; set; }
  }
}
