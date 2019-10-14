using SMEIoT.Core.Entities;

namespace SMEIoT.Web.ApiModels
{
  public class SensorDetailsApiModel : BasicSensorApiModel
  {
    SensorValuesApiModel Values { get; set; }
    public SensorDetailsApiModel(Sensor sensor, SensorValuesApiModel values) : base(sensor)
    {
      Values = values;
    }
  }
}
