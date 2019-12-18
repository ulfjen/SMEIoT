using System.Threading.Tasks;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Interfaces;
using NodaTime;

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
  }
}
