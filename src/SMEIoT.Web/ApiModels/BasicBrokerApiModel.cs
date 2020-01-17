using System;
using System.Collections.Generic;
using NodaTime;
using Newtonsoft.Json;

namespace SMEIoT.Web.ApiModels
{
  public class BasicBrokerApiModel 
  {
    public bool Running { get; set; } = default;
    
    public Instant? LastUpdatedAt { get; set; }

    public double? Min1 { get; set; }

    public double? Min5 { get; set; }

    public double? Min15 { get; set; }
    
    [JsonProperty(Required = Required.DisallowNull)]
    public string Host { get; set; }

    public int Port { get; set; }

    public BasicBrokerApiModel(bool running, Instant? lastUpdatedAt, Tuple<double?, double?, double?> loads, (string, int) clientInfo)
    {
      Running = running;
      LastUpdatedAt = lastUpdatedAt;
      Min1 = loads.Item1;
      Min5 = loads.Item2;
      Min15 = loads.Item3;
      (Host, Port) = clientInfo;
    }
  }
}
