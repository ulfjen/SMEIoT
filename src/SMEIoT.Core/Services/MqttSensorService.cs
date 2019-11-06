using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using NodaTime;
using SMEIoT.Core.Interfaces;

namespace SMEIoT.Core.Services
{
  public class MqttSensorService : IMqttSensorService
  {
    private ConcurrentDictionary<string, Instant> _sensorNames = new ConcurrentDictionary<string, Instant>();
    private readonly IClock _clock;

    private static Duration ExpiredPeriod = Duration.FromMinutes(15);

    public MqttSensorService(IClock clock)
    {
      _clock = clock;
    }

    public IEnumerable<string> ListSensorNames(string pattern)
    {
      var keys = _sensorNames.Keys;
      var expired = new List<string>();
      var result = new List<string>();
      Instant instant;
      foreach (var key in keys)
      {
        _sensorNames.TryGetValue(key, out instant);
        if (instant + ExpiredPeriod >= _clock.GetCurrentInstant())
        {
           result.Add(key);
        }
        else
        {
          expired.Add(key);
        }
      }
      foreach (var key in expired)
      {
        _sensorNames.TryRemove(key, out instant);
      }
      return result;
    }

    public Task<bool> RegisterSensorByName(string sensorName)
    {
      return Task.FromResult(_sensorNames.TryAdd(sensorName, _clock.GetCurrentInstant()));
    }
  }
}
