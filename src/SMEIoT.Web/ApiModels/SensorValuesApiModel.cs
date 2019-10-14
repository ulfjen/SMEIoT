using System.Collections.Generic;
using NodaTime;

namespace SMEIoT.Web.ApiModels
{
  public class SensorValuesApiModel
  {
    public IEnumerable<double> Values { get; set; }
    public Instant StartedAt { get; set; }
    public Duration Interval { get; set; }

    public SensorValuesApiModel(IEnumerable<double> values, Instant startedAt, Duration interval)
    {
      Values = values;
      StartedAt = startedAt;
      Interval = interval;
    }
  }
}
