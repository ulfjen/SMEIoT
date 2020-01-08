using Newtonsoft.Json;

namespace SMEIoT.Web.ApiModels
{
  public class DeviceConfigSuggestApiModel
  {
    [JsonProperty(Required = Required.DisallowNull)]
    public string? DeviceName { get; set; }

    [JsonProperty(Required = Required.DisallowNull)]
    public string? Key { get; set; }

    [JsonProperty(Required = Required.DisallowNull)]
    public string? ContinuedConfigurationDevice { get; set; }

    public DeviceConfigSuggestApiModel(string? deviceName = null, string? key = null, string? continuedConfigurationDevice = null)
    {
      DeviceName = deviceName;
      Key = key;
      ContinuedConfigurationDevice = continuedConfigurationDevice;
    }
  }
}
