using System.Collections.Generic;
using NodaTime;
using Newtonsoft.Json;

namespace SMEIoT.Web.ApiModels
{
  public class SensorValuesApiModel
  {
    [JsonProperty(Required = Required.DisallowNull)]
    public IEnumerable<double> Values { get; set; }
    
    [JsonProperty(Required = Required.DisallowNull)]
    public Instant StartedAt { get; set; }
    
    [JsonProperty(Required = Required.DisallowNull)]
    public Duration Interval { get; set; }

    public SensorValuesApiModel(IEnumerable<double> values, Instant startedAt, Duration interval)
    {
      Values = values;
      StartedAt = startedAt;
      Interval = interval;
    }
  }
}
