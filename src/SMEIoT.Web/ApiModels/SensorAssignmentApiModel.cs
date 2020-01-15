using Newtonsoft.Json;
using SMEIoT.Core.Entities;

namespace SMEIoT.Web.ApiModels
{
  public class SensorAssignmentApiModel
  {
    [JsonProperty(Required = Required.DisallowNull)]
    public string SensorName { get; set; }
    
    [JsonProperty(Required = Required.DisallowNull)]
    public string UserName { get; set; }

    public SensorAssignmentApiModel(string sensorName, string userName)
    {
      SensorName = sensorName;
      UserName = userName;
    }
  }
}
