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

    IAsyncEnumerable<Sensor> ListSensorsByDeviceAsync(Device device);
    Task CreateSensorByDeviceAndNameAsync(Device device, string sensorName);
    Task<Sensor> GetSensorByDeviceAndNameAsync(Device device, string sensorName);
    IAsyncEnumerable<(double value, Instant createdAt)> GetNumberTimeSeriesByDeviceAndSensorAsync(Device device, Sensor sensor, Instant startedAt, Duration duration);
  }
}
