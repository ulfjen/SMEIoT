using System.Collections.Generic;
using NodaTime;
using SMEIoT.Core.Entities;
using Newtonsoft.Json;

namespace SMEIoT.Web.ApiModels
{
  public class BrokerDetailsApiModel 
  {
    [JsonProperty(Required = Required.DisallowNull)]
    public IDictionary<string, string> Statistics { get; set; }
    
    [JsonProperty(Required = Required.DisallowNull)]
    public bool Running { get; set; }
    
    [JsonProperty(Required = Required.DisallowNull)]
    public Instant? LastUpdatedAt { get; set; }

    public BrokerDetailsApiModel(IEnumerable<KeyValuePair<string, string>> values, bool running, Instant? lastUpdatedAt)
    {
      Statistics = new Dictionary<string, string>();
      foreach (var v in values) 
      {
        Statistics[v.Key] = v.Value;
      }
      Running = running;
      LastUpdatedAt = lastUpdatedAt;
    }
  }
}
