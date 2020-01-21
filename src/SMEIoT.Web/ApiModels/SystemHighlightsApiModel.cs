using Newtonsoft.Json;
using SMEIoT.Core.Entities;

namespace SMEIoT.Web.ApiModels
{
  public class SystemHighlightsApiModel
  {
    [JsonProperty(Required = Required.DisallowNull)]
    public int UserCount { get; set; }

    [JsonProperty(Required = Required.DisallowNull)]
    public int AdminCount { get; set; }

    [JsonProperty(Required = Required.DisallowNull)]
    public int ConnectedSensorCount { get; set; }

    [JsonProperty(Required = Required.DisallowNull)]
    public int SensorCount { get; set; }

    [JsonProperty(Required = Required.DisallowNull)]
    public int ConnectedDeviceCount { get; set; }

    [JsonProperty(Required = Required.DisallowNull)]
    public int DeviceCount { get; set; }

    public SystemHighlightsApiModel(SystemHighlights highlights)
    {
      UserCount = highlights.UserCount;
      AdminCount = highlights.AdminCount;
      ConnectedSensorCount = highlights.ConnectedSensorCount;
      SensorCount = highlights.SensorCount;
      ConnectedDeviceCount = highlights.ConnectedDeviceCount;
      DeviceCount = highlights.DeviceCount;
    }
  }
}
