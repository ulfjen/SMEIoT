using System.Collections.Generic;
using Newtonsoft.Json;

namespace SMEIoT.Web.ApiModels
{
  public class SensorCandidatesApiModel
  {
    [JsonProperty(Required = Required.DisallowNull)]
    public IEnumerable<string> Names { get; set; }

    public SensorCandidatesApiModel(IEnumerable<string> names)
    {
      Names = names;
    }
  }
}
