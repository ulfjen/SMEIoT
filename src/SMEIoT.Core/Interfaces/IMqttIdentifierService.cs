using System;
using System.Collections.Generic;

namespace SMEIoT.Core.Interfaces
{
  public interface IMqttIdentifierService
  {
    /// <summary>
    /// Registers a sensor name for retrieval later
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool RegisterSensorNameWithDeviceName(string name, string deviceName);

    IEnumerable<string> ListSensorNamesByDeviceName(string deviceName);


    /// <summary>
    /// Registers a device name for retrieval later
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool RegisterDeviceName(string name);


    IEnumerable<string> ListDeviceNames();
  }
}
