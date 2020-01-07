using System.Collections.Generic;
using Newtonsoft.Json;

namespace SMEIoT.Web.ApiModels
{
  public class BasicDeviceApiModelList
  {
    [JsonProperty(Required = Required.DisallowNull)]
    public IEnumerable<BasicDeviceApiModel> Devices { get; set; }

    public BasicDeviceApiModelList(IEnumerable<BasicDeviceApiModel> devices)
    {
      Devices = devices;
    }
  }
}
