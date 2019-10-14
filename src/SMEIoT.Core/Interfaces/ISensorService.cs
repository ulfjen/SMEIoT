using System.Collections.Generic;
using System.Threading.Tasks;
using NodaTime;
using SMEIoT.Core.Entities;

namespace SMEIoT.Core.Interfaces
{
  public interface ISensorService
  {
    Task<bool> CreateSensor(string sensorName);
    Task<Sensor> GetSensorByName(string sensorName);
    IAsyncEnumerable<double> GetSensorValues(string sensorName, Instant startedAt, Duration duration);
  }
}
