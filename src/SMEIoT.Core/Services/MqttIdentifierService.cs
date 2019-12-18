using System.Collections.Concurrent;
using System.Collections.Generic;
using NodaTime;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace SMEIoT.Core.Services
{
  public class MqttIdentifierService : IMqttIdentifierService
  {
    private readonly AutoExpiredSet<string> _deviceNames = new AutoExpiredSet<string>();
    private readonly ConcurrentDictionary<string, AutoExpiredSet<string>> _sensorNames = new ConcurrentDictionary<string, AutoExpiredSet<string>>();
    private readonly IClock _clock;

    public MqttIdentifierService(IClock clock)
    {
      _clock = clock;
    }

    public bool RegisterDeviceName(string name)
    {
      if (string.IsNullOrEmpty(name)) { return false; }
      return _deviceNames.TryAdd(name, _clock.GetCurrentInstant());
    }

    public IEnumerable<string> ListDeviceNames()
    {
      return _deviceNames.List(_clock.GetCurrentInstant());
    }

    public bool RegisterSensorNameWithDeviceName(string name, string deviceName)
    {
      if (!_sensorNames.ContainsKey(deviceName))
      {
        _sensorNames[deviceName] = new AutoExpiredSet<string>();
      }
      return _sensorNames[deviceName].TryAdd(name, _clock.GetCurrentInstant());
    }

    public IEnumerable<string> ListSensorNamesByDeviceName(string deviceName)
    {
      if (_sensorNames.ContainsKey(deviceName))
      {
        return _sensorNames[deviceName].List(_clock.GetCurrentInstant());
      }
      return System.Array.Empty<string>();
    }
  }
}
