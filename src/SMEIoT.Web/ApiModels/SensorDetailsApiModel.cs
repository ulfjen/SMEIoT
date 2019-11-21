using SMEIoT.Core.Entities;
using Newtonsoft.Json;

namespace SMEIoT.Web.ApiModels
{
  public class SensorDetailsApiModel : BasicSensorApiModel
  {
    [JsonProperty(Required = Required.DisallowNull)]
    SensorValuesApiModel Values { get; set; }
    public SensorDetailsApiModel(Sensor sensor, SensorValuesApiModel values) : base(sensor)
    {
      Values = values;
    }
  }
}
