using System.Collections.Generic;
using Newtonsoft.Json;

namespace SMEIoT.Web.ApiModels
{
  public class BrokerStatisticsApiModel 
  {
    [JsonProperty(Required = Required.DisallowNull)]
    public IDictionary<string, string> Statistics { get; set; }
    
    public BrokerStatisticsApiModel(IEnumerable<KeyValuePair<string, string>> values)
    {
      Statistics = new Dictionary<string, string>();
      foreach (var v in values) 
      {
        Statistics[v.Key] = v.Value;
      }
    }
  }
}
