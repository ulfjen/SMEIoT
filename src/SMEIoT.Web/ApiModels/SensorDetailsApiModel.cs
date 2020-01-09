using SMEIoT.Core.Entities;
using System.Collections.Generic;
using NodaTime;
using Newtonsoft.Json;

namespace SMEIoT.Web.ApiModels
{
  public class SensorDetailsApiModel : BasicSensorApiModel
  {
    [JsonProperty(Required = Required.DisallowNull)]
    public string DeviceName { get; set; }

    [JsonProperty(Required = Required.DisallowNull)]
    public IEnumerable<NumberTimeSeriesApiModel> Data { get; set; }
    
    public Instant StartedAt { get; set; }

    public SensorDetailsApiModel(Sensor sensor, IEnumerable<(double, Instant)> values) : base(sensor)
    {
      DeviceName = sensor.Device.Name;

      var intermediate = new List<NumberTimeSeriesApiModel>();
      foreach (var item in values)
      {
        intermediate.Add(new NumberTimeSeriesApiModel(item));
      }
      Data = intermediate;
    }
  }
}
