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
    
    public Instant? StartedAt { get; set; }
    public Duration? Duration { get; set; }

    public SensorDetailsApiModel(Sensor sensor, IList<(double, Instant)> values) : base(sensor)
    {
      DeviceName = sensor.Device.Name;

      if (values.Count > 0) {
        StartedAt = values[0].Item2;
        Duration = values[values.Count-1].Item2 - StartedAt;
      }
      var intermediate = new List<NumberTimeSeriesApiModel>();
      foreach (var item in values)
      {
        intermediate.Add(new NumberTimeSeriesApiModel(item));
      }
      Data = intermediate;
    }
  }
}
