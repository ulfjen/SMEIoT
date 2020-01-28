using System.Collections.Generic;
using System.Linq;
using SMEIoT.Core.Interfaces;
using NodaTime;
using Microsoft.EntityFrameworkCore;

namespace SMEIoT.Core.Jobs
{
  public sealed class ToggleMqttEntityStatusJob : IToggleMqttEntityStatusJob
  {
    private readonly IClock _clock;
    private readonly IApplicationDbContext _dbContext;
    
    public ToggleMqttEntityStatusJob(IClock clock, IApplicationDbContext dbContext)
    {
      _clock = clock;
      _dbContext = dbContext;
    }
    
    public void ScanAndToggleMqttEntityConnectedStatus()
    {
      var inactiveBound = _clock.GetCurrentInstant() - Duration.FromMinutes(5);

      var deviceIds = new HashSet<long>();
      var sensors = _dbContext.Sensors.Include(s => s.Device).Where(s => s.Connected && s.LastMessageAt < inactiveBound);
      foreach (var s in sensors) {
        deviceIds.Add(s.Device.Id);
        s.Connected = false;
        _dbContext.Sensors.Update(s);
      }
      _dbContext.SaveChanges();

      var devices = _dbContext.Devices.Where(d => d.Connected && deviceIds.Contains(d.Id) && d.Sensors.All(s => !s.Connected));
      foreach (var d in devices) {
        d.Connected = false;
        _dbContext.Devices.Update(d);
      }
      _dbContext.SaveChanges();
    }
  }
}
