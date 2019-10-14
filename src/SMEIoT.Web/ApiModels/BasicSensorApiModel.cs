using SMEIoT.Core.Entities;

namespace SMEIoT.Web.ApiModels
{
  public class BasicSensorApiModel
  {
    public string sensorName { get; set; }

    public BasicSensorApiModel(Sensor sensor)
    {
      sensorName = sensor.Name;
    }
  }
}
