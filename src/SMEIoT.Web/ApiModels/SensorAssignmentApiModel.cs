using Newtonsoft.Json;
using SMEIoT.Core.Entities;

namespace SMEIoT.Web.ApiModels
{
  public class SensorAssignmentApiModel
  {
    [JsonProperty(Required = Required.DisallowNull)]
    public string SensorName { get; set; }

    [JsonProperty(Required = Required.DisallowNull)]
    public BasicUserApiModel User { get; set; }

    public SensorAssignmentApiModel(string sensorName, BasicUserApiModel user)
    {
      SensorName = sensorName;
      User = user;
    }
  }
}
