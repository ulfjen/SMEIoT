using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using NodaTime;

namespace SMEIoT.Web.ApiModels
{
  public class BasicBrokerApiModel 
  {
    [JsonProperty(Required = Required.DisallowNull)]
    public bool Running { get; set; }
    
    [JsonProperty(Required = Required.DisallowNull)]
    public Instant? LastUpdatedAt { get; set; }

    [JsonProperty(Required = Required.DisallowNull)]
    public double? Min1 { get; set; }

    [JsonProperty(Required = Required.DisallowNull)]
    public double? Min5 { get; set; }

    [JsonProperty(Required = Required.DisallowNull)]
    public double? Min15 { get; set; }

    public BasicBrokerApiModel(bool running, Instant? lastUpdatedAt, Tuple<double?, double?, double?> loads)
    {
      Running = running;
      LastUpdatedAt = lastUpdatedAt;
      Min1 = loads.Item1;
      Min5 = loads.Item2;
      Min15 = loads.Item3;
    }
  }
}
