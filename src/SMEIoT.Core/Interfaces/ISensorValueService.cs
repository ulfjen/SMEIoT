using SMEIoT.Core.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using NodaTime;

namespace SMEIoT.Core.Interfaces
{
  public interface ISensorValueService
  {
    Task AddSensorValueAsync(Sensor sensor, double value, Instant instant);
    IAsyncEnumerable<(double value, Instant createdAt)> GetNumberTimeSeriesBySensorAsync(Sensor sensor, Instant startedAt, Duration duration);
  }
}
