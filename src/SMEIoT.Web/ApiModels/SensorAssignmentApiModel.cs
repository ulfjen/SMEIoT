using Newtonsoft.Json;

namespace SMEIoT.Web.ApiModels
{
  public class SensorAssignmentApiModel
  {
    [JsonProperty(Required = Required.DisallowNull)]
    public string Name { get; set; }
    
    [JsonProperty(Required = Required.DisallowNull)]
    public string UserName { get; set; }
    
    [JsonProperty(Required = Required.DisallowNull)]
    public int AssignmentCount { get; set; }
  }
}
