using Newtonsoft.Json;
using SMEIoT.Core.Entities;

namespace SMEIoT.Web.ApiModels
{
  public class SensorAssignmentApiModel
  {
    [JsonProperty(Required = Required.DisallowNull)]
    public string SensorName { get; set; }

    [JsonProperty(Required = Required.DisallowNull)]
    public AdminUserApiModel User { get; set; }

    public SensorAssignmentApiModel(string sensorName, AdminUserApiModel user)
    {
      SensorName = sensorName;
      User = user;
    }
  }
}
