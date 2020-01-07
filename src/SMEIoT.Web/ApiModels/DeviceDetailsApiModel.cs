using SMEIoT.Core.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;

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
