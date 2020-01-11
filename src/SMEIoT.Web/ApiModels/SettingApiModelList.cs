using System.Collections.Generic;
using Newtonsoft.Json;

namespace SMEIoT.Web.ApiModels
{
  public class SettingApiModelList
  {
    [JsonProperty(Required = Required.DisallowNull)]
    public IEnumerable<SettingApiModel> Settings { get; set; }

    public SettingApiModelList(IEnumerable<SettingApiModel> settings)
    {
      Settings = settings;
    }
  }
}
