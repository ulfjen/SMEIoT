using Newtonsoft.Json;
using SMEIoT.Core.Entities;

namespace SMEIoT.Web.ApiModels
{
  public class BasicSensorApiModel
  {
    [JsonProperty(Required = Required.DisallowNull)]
    public string SensorName { get; set; }

    public SensorStatus Status { get; set; }

    public BasicSensorApiModel(Sensor sensor)
    {
      SensorName = sensor.Name;
      Status = sensor.Connected ? SensorStatus.Connected : SensorStatus.NotConnected;
    }

    public BasicSensorApiModel(string sensorName)
    {
      SensorName = sensorName;
      Status = SensorStatus.NotRegistered;
    }
  }
}
