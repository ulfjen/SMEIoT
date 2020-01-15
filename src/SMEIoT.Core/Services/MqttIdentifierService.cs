using System.Collections.Concurrent;
using System.Collections.Generic;
using NodaTime;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Interfaces;
using System.Threading.Tasks;
using System.Linq;
using SMEIoT.Core.Exceptions;

namespace SMEIoT.Core.Services
{
  public class MqttIdentifierService : IMqttIdentifierService
  {
    // TODO: replace with better data structure
    private readonly AutoExpiredSet<string> _deviceNames = new AutoExpiredSet<string>();
    private readonly ConcurrentDictionary<string, AutoExpiredSet<string>> _sensorNames = new ConcurrentDictionary<string, AutoExpiredSet<string>>();
    private readonly IClock _clock;

    public MqttIdentifierService(IClock clock)
    {
      _clock = clock;
    }

    public Task RegisterDeviceNameAsync(string name)
    {
      if (string.IsNullOrEmpty(name))
      {
        throw new InvalidArgumentException("device name can't be empty", nameof(name)); 
      }
      _deviceNames.TryAdd(name, _clock.GetCurrentInstant());
      return Task.CompletedTask;
    }

    public Task<IEnumerable<string>> ListDeviceNamesAsync()
    {
      return Task.FromResult(_deviceNames.List(_clock.GetCurrentInstant()));
    }

    public Task RegisterSensorNameWithDeviceNameAsync(string name, string deviceName)
    {
      if (string.IsNullOrEmpty(name))
      {
        throw new InvalidArgumentException("sensor name can't be empty.", nameof(name));
      }
      if (!_deviceNames.ContainsKey(deviceName))
      {
        throw new InvalidArgumentException($"unknown device name {deviceName}.", nameof(deviceName));
      }
      if (!_sensorNames.ContainsKey(deviceName))
      {
        _sensorNames[deviceName] = new AutoExpiredSet<string>();
      }
      _sensorNames[deviceName].TryAdd(name, _clock.GetCurrentInstant());
      return Task.CompletedTask;
    }

    public Task<IEnumerable<string>> ListSensorNamesByDeviceNameAsync(string deviceName)
    {
      if (_sensorNames.ContainsKey(deviceName))
      {
        return Task.FromResult(_sensorNames[deviceName].List(_clock.GetCurrentInstant()));
      }
      return Task.FromResult(new List<string>().AsEnumerable());
    }
  }
}
