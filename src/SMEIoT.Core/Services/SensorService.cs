using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Exceptions;
using SMEIoT.Core.Interfaces;

namespace SMEIoT.Core.Services
{
  public class SensorService : ISensorService
  {
    private readonly IApplicationDbContext _dbContext;

    public SensorService(IApplicationDbContext dbContext)
    {
      _dbContext = dbContext;
    } 
    
    public async Task<bool> CreateSensor(string sensorName)
    {
      _dbContext.Sensors.Add(new Sensor { Name = sensorName, NormalizedName = Sensor.NormalizeName(sensorName) });
      await _dbContext.SaveChangesAsync();
      return true;
    }

    public async Task<Sensor> GetSensorByName(string sensorName)
    {
      var sensor = await _dbContext.Sensors.Where(s => s.NormalizedName == Sensor.NormalizeName(sensorName)).FirstOrDefaultAsync();
      if (sensor == null)
      {
        throw new EntityNotFoundException("cannot find the sensor.", "sensorName");
      }

      return sensor;
    }

    public async IAsyncEnumerable<double> GetSensorValues(string sensorName, Instant startedAt, Duration duration)
    {
      var sensor = await _dbContext.Sensors.Where(s => s.NormalizedName == Sensor.NormalizeName(sensorName)).FirstOrDefaultAsync();
      if (sensor == null)
      {
        throw new EntityNotFoundException("cannot find the sensor.", "sensorName");
      }

      foreach (var d in new[] { 32.4, 72.0, 32.1, 72, 32.1, 72.0 })
      {
        yield return d;
      }
    }


  }
}
