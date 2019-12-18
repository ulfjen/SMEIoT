using SMEIoT.Core.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;
using NodaTime;

namespace SMEIoT.Web.ApiModels
{
  public class NumberTimeSeriesApiModel
  {
    [JsonProperty(Required = Required.DisallowNull)]
    public double Value { get; set; }
    
    [JsonProperty(Required = Required.DisallowNull)]
    public Instant CreatedAt { get; set; }

    public NumberTimeSeriesApiModel(double value, Instant createdAt)
    {
      Value = value;
      CreatedAt = createdAt;
    }

    public NumberTimeSeriesApiModel((double, Instant) t)
      : this(t.Item1, t.Item2)
    {
    }
  }
}
