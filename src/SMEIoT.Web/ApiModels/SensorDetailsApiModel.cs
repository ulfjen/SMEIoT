using SMEIoT.Core.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;
using NodaTime;

namespace SMEIoT.Web.ApiModels
{
  public class SensorDetailsApiModel : BasicSensorApiModel
  {
    // [JsonProperty(Required = Required.DisallowNull)]
    // SensorValuesApiModel Values { get; set; } = null!;

    [JsonProperty(Required = Required.DisallowNull)]
    public IEnumerable<double> Values { get; set; }
    
    [JsonProperty(Required = Required.DisallowNull)]
    public Instant StartedAt { get; set; }
    
    [JsonProperty(Required = Required.DisallowNull)]
    public Duration Interval { get; set; }

    public SensorDetailsApiModel(Sensor sensor, SensorValuesApiModel value) : base(sensor)
    {
      Values = value.Values;
      StartedAt = value.StartedAt;
      Interval = value.Interval;
    }
  }
}
