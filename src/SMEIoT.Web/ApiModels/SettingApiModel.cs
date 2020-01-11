using System.Collections.Generic;
using NodaTime;
using SMEIoT.Core.Entities;
using Newtonsoft.Json;

namespace SMEIoT.Web.ApiModels
{
  public class SettingApiModel
  {
    [JsonProperty(Required = Required.DisallowNull)]
    public string Name { get; set; }

    [JsonProperty(Required = Required.DisallowNull)]
    public string Description { get; set; }
    
    [JsonProperty(Required = Required.DisallowNull)]
    public string Type { get; set; }
    
    [JsonProperty(Required = Required.DisallowNull)]
    public object DefaultValue { get; set; }
    
    [JsonProperty(Required = Required.DisallowNull)]
    public object Value { get; set; }

    public SettingApiModel(SettingItemDescriptor descriptor)
    {
      Name = descriptor.Name;
      Description = descriptor.Description;
      Type = descriptor.Type;
      DefaultValue = descriptor.DefaultValue;
      Value = descriptor.Value; 
    }
  }
}
