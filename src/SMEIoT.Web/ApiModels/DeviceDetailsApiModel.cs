using SMEIoT.Core.Entities;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SMEIoT.Web.ApiModels
{
  public class DeviceDetailsApiModel : BasicDeviceApiModel
  {
    [JsonProperty(Required = Required.DisallowNull)]
    public IEnumerable<SensorDetailsApiModel> Sensors { get; }

    public DeviceDetailsApiModel(
      Device device,
      IEnumerable<SensorDetailsApiModel> sensors,
      MqttBrokerConnectionInformation info)
      : base(device, info)
    {
      Sensors = sensors;
    }
  }
}
