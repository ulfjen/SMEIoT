using SMEIoT.Core.Entities;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SMEIoT.Web.ApiModels
{
  public class DeviceDetailsApiModel : BasicDeviceApiModel
  {
    [JsonProperty(Required = Required.DisallowNull)]
    public IEnumerable<BasicSensorApiModel> Sensors { get; }

    public DeviceDetailsApiModel(Device device, IEnumerable<BasicSensorApiModel> sensors)
      : base(device)
    {
      Sensors = sensors;
    }
  }
}
