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
    Task RegisterSensorNameWithDeviceNameAsync(string name, string deviceName);

    Task<IEnumerable<string>> ListSensorNamesByDeviceNameAsync(string deviceName);


    /// <summary>
    /// Registers a device name for retrieval later
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Task RegisterDeviceNameAsync(string name);


    Task<IEnumerable<string>> ListDeviceNamesAsync();
  }
}
