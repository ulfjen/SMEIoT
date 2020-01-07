using SMEIoT.Core.Entities;
using Newtonsoft.Json;

namespace SMEIoT.Web.ApiModels
{
  public class BasicSensorApiModel
  {
    [JsonProperty(Required = Required.DisallowNull)]
    public string SensorName { get; set; }

    [JsonProperty(Required = Required.DisallowNull)]
    public string DeviceName { get; set; }

    public BasicSensorApiModel(Sensor sensor)
    {
      SensorName = sensor.Name;
      DeviceName = sensor.Device.Name;
    }

    public BasicSensorApiModel(string sensorName, string deviceName)
    {
      SensorName = sensorName;
      DeviceName = deviceName;
    }
  }
}
