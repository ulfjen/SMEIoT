using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMEIoT.Core.Interfaces
{
  public interface IMqttSensorService
  {
    /// <summary>
    /// Registers a sensor name for retrieval later
    /// </summary>
    /// <param name="sensorName"></param>
    /// <returns></returns>
    Task<bool> RegisterSensorByName(string sensorName);
    IEnumerable<string> ListSensorNames(string pattern);
  }
}
