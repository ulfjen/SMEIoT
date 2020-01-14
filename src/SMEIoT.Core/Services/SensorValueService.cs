using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Interfaces;
using NodaTime;
using Microsoft.EntityFrameworkCore;

namespace SMEIoT.Core.Services
{
  public class SensorValueService : ISensorValueService
  {
    private readonly IApplicationDbContext _dbContext;

    public SensorValueService(IApplicationDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public async Task AddSensorValueAsync(Sensor sensor, double value, Instant instant)
    {
      _dbContext.SensorValues.Add(new SensorValue { Sensor = sensor, Value = value, CreatedAt = instant });
      await _dbContext.SaveChangesAsync();
    }

    public async IAsyncEnumerable<(double value, Instant createdAt)> GetNumberTimeSeriesBySensorAsync(Sensor sensor, Instant startedAt, Duration duration)
    {
      var query = from sv in _dbContext.SensorValues
                  where sv.SensorId == sensor.Id && sv.CreatedAt >= startedAt && sv.CreatedAt < startedAt + duration
                  select sv;
      await foreach (var sv in query.AsAsyncEnumerable()) {
        yield return (sv.Value, sv.CreatedAt);
      }
    }
  }
}
