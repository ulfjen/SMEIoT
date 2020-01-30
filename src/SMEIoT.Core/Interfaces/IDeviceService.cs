using System.Collections.Generic;
using System.Threading.Tasks;
using NodaTime;
using SMEIoT.Core.Entities;

namespace SMEIoT.Core.Interfaces
{
  public interface IDeviceService
  {
    Task<string> BootstrapDeviceWithPreSharedKeyAsync(string identity, string key);
    Task<Device> GetDeviceByNameAsync(string deviceName);
    Task<Device?> GetARandomUnconnectedDeviceAsync();
    IAsyncEnumerable<Device> ListDevicesAsync(int offset, int limit);

    IAsyncEnumerable<Sensor> ListSensorsAsync(int offset, int limit);
    Task<int> NumberOfSensorsAsync();

    IAsyncEnumerable<Sensor> ListSensorsByDeviceAsync(Device device);
    Task CreateSensorByDeviceAndNameAsync(Device device, string sensorName);
    Task<Sensor> GetSensorByDeviceAndNameAsync(Device device, string sensorName);
    // will also delete user_sensors records
    Task RemoveSensorByDeviceAndNameAsync(Device device, string sensorName);
    // will also delete sensors, user_sensors records
    Task RemoveDeviceByNameAsync(string deviceName);
  
    Task UpdateDeviceConfigAsync(Device device, string config);

    Task UpdateDeviceTimestampsAndStatusAsync(Device device, Instant receivedAt);
    Task UpdateSensorAndDeviceTimestampsAndStatusAsync(Sensor sensor, Instant receivedAt);
  }
}
