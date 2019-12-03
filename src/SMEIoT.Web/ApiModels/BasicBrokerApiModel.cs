using NodaTime;
using Newtonsoft.Json;

namespace SMEIoT.Web.ApiModels
{
  public class BasicBrokerApiModel 
  {
    [JsonProperty(Required = Required.DisallowNull)]
    public bool Running { get; set; }
    
    [JsonProperty(Required = Required.DisallowNull)]
    public Instant? LastUpdatedAt { get; set; }

    public BasicBrokerApiModel(bool running, Instant? lastUpdatedAt, IEnumerable<KeyValuePair<string, string>> statistics)
    {
      Running = running;
      LastUpdatedAt = lastUpdatedAt;
    }
  }
}
