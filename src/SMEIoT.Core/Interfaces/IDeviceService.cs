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
    IAsyncEnumerable<Device> ListDevicesAsync(int start, int limit);
  }
}
