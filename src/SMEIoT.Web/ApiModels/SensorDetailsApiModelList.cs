using System.Collections.Generic;
using Newtonsoft.Json;

namespace SMEIoT.Web.ApiModels
{
  public class SensorDetailsApiModelList
  {
    [JsonProperty(Required = Required.DisallowNull)]
    public IEnumerable<SensorDetailsApiModel> Sensors { get; set; }

    public int Total { get; set; }

    public SensorDetailsApiModelList(IEnumerable<SensorDetailsApiModel> sensors, int total)
    {
      Sensors = sensors;
      Total = total;
    }
  }
}
