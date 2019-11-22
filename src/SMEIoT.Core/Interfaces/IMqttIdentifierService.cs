using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMEIoT.Core.Interfaces
{
  public interface IMqttIdentifierService
  {
    /// <summary>
    /// Registers a sensor name for retrieval later
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Task<bool> RegisterSensorNameAsync(string name);

    IEnumerable<string> ListSensorNames();
    
    /// <summary>
    /// Registers a device name for retrieval later
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Task<bool> RegisterDeviceNameAsync(string name);

    IEnumerable<string> ListDeviceNames();
  }
}
