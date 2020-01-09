using Newtonsoft.Json;
using SMEIoT.Core.Entities;

namespace SMEIoT.Web.ApiModels
{
  public enum SensorStatus
  {
    // we got a message from mqtt
    NotRegistered,
    
    // our sensor is registered in db but not connected anymore
    NotConnected, 
    
    // registered in db and runs fine 
    Connected
  }

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
