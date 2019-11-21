using SMEIoT.Core.Entities;
using Newtonsoft.Json;

namespace SMEIoT.Web.ApiModels
{
  public class BasicSensorApiModel
  {
    [JsonProperty(Required = Required.DisallowNull)]
    public string SensorName { get; set; }

    public BasicSensorApiModel(Sensor sensor)
    {
      SensorName = sensor.Name;
    }
  }
}
