using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Exceptions;
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
                  orderby sv.CreatedAt
                  select sv;
      await foreach (var sv in query.AsNoTracking().AsAsyncEnumerable()) {
        yield return (sv.Value, sv.CreatedAt);
      }
    }

    public async IAsyncEnumerable<(double value, Instant createdAt)> GetLastSecondsOfValuesBySensorAsync(Sensor sensor, int seconds)
    {
      if (seconds < 0) {
        throw new InvalidArgumentException("Seconds must not be negative.", nameof(seconds));
      }
      var query = from sv in _dbContext.SensorValues
                  where sv.SensorId == sensor.Id
                  orderby sv.CreatedAt descending
                  select sv;
      var v = await query.FirstOrDefaultAsync();
      if (v == null) {
        yield break;
      }
      var lastMessageTime = v.CreatedAt;
      var duration = Duration.FromSeconds(seconds);

      query = from sv in _dbContext.SensorValues
              where sv.SensorId == sensor.Id && sv.CreatedAt >= lastMessageTime - duration
              orderby sv.CreatedAt
              select sv;
      await foreach (var sv in query.AsNoTracking().AsAsyncEnumerable()) {
        yield return (sv.Value, sv.CreatedAt);
      }
    }


    public async IAsyncEnumerable<(Sensor sensor, double value, Instant createdAt)> GetLastNumberOfValuesBySensorsAsync(IEnumerable<Sensor> sensors, int count)
    {
      if (count < 0) {
        throw new InvalidArgumentException("Count must not be negative.", nameof(count));
      }
      var sensorIds = new List<long>();
      var sensorById = new Dictionary<long, Sensor>();
      foreach (var s in sensors) {
        sensorIds.Add(s.Id);
        sensorById[s.Id] = s;
      }
      if (sensorIds.Count != 0) {
        var sensorIdsSet = string.Join(',', sensorIds);
        var enumerable = _dbContext.SensorValues.FromSqlRaw(
          $"SELECT * FROM (SELECT *, ROW_NUMBER() OVER (PARTITION BY sensor_id ORDER BY created_at DESC) AS cnt FROM sensor_values) AS x WHERE cnt <= {count} AND sensor_id IN ({sensorIdsSet})")
          .OrderBy(x => x.CreatedAt)
          .AsNoTracking()
          .AsAsyncEnumerable();
    
        await foreach (var sv in enumerable) {
          yield return (sensorById[sv.SensorId], sv.Value, sv.CreatedAt);
        }
      }
    }
  }
}
