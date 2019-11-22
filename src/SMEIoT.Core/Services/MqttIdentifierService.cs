using System.Collections.Generic;
using System.Threading.Tasks;
using NodaTime;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Interfaces;

namespace SMEIoT.Core.Services
{
  public class MqttIdentifierService : IMqttIdentifierService
  {
    private readonly AutoExpiredSet<string> _deviceNames = new AutoExpiredSet<string>();
    private readonly AutoExpiredSet<string> _sensorNames = new AutoExpiredSet<string>();
    private readonly IClock _clock;

    public MqttIdentifierService(IClock clock)
    {
      _clock = clock;
    }

    public Task<bool> RegisterSensorNameAsync(string name)
    {
      return Task.FromResult(_sensorNames.TryAdd(name, _clock.GetCurrentInstant()));
    }

    public IEnumerable<string> ListSensorNames()
    {
      return _sensorNames.List(_clock.GetCurrentInstant());
    }

    public Task<bool> RegisterDeviceNameAsync(string name)
    {
      return Task.FromResult(_deviceNames.TryAdd(name, _clock.GetCurrentInstant()));
    }

    public IEnumerable<string> ListDeviceNames()
    {
      return _deviceNames.List(_clock.GetCurrentInstant());
    }
  }
}
