using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SMEIoT.Core.Entities;

namespace SMEIoT.Core.Interfaces
{
  public interface IApplicationDbContext : IDisposable
  {
    DbSet<User> Users { get; set; }
    DbSet<Sensor> Sensors { get; set; }
    DbSet<SensorValue> SensorValues { get; set; }
    DbSet<UserSensor> UserSensors { get; set; }
    DbSet<IdentityUserRole<long>> UserRoles { get; set; }
    DbSet<SettingItem> SettingItems { get; set; }
    DbSet<Device> Devices { get; set; }
    DatabaseFacade Database { get; }

    int SaveChanges();
    Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
      CancellationToken cancellationToken = default(CancellationToken));

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
  }

}
