using System.Collections.Generic;
using Newtonsoft.Json;

namespace SMEIoT.Web.ApiModels
{
  public class DeviceApiModelList
  {
    [JsonProperty(Required = Required.DisallowNull)]
    public IEnumerable<DeviceApiModel> Devices { get; set; }

    public DeviceApiModelList(IEnumerable<DeviceApiModel> devices)
    {
      Devices = devices;
    }
  }
}
