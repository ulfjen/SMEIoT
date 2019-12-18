using SMEIoT.Core.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;
using NodaTime;

namespace SMEIoT.Web.ApiModels
{
  public class SensorDetailsApiModel : BasicSensorApiModel
  {
    [JsonProperty(Required = Required.DisallowNull)]
    public IEnumerable<NumberTimeSeriesApiModel> Data { get; set; }
    
    [JsonProperty(Required = Required.DisallowNull)]
    public Instant StartedAt { get; set; }

    public SensorDetailsApiModel(Sensor sensor, IEnumerable<(double, Instant)> values) : base(sensor)
    {
      var intermediate = new List<NumberTimeSeriesApiModel>();
      foreach (var item in values)
      {
        intermediate.Add(new NumberTimeSeriesApiModel(item));
      }
      Data = intermediate;
    }
  }
}
