using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SMEIoT.Core.Entities;

namespace SMEIoT.Core.Interfaces
{
  public interface IApplicationDbContext
  {
    DbSet<User> Users { get; set; }
    DbSet<Sensor> Sensors { get; set; }
    DbSet<SensorValue> SensorValues { get; set; }
    DbSet<UserSensor> UserSensors { get; set; }
    DbSet<IdentityUserRole<long>> UserRoles { get; set; }
    DbSet<Device> Devices { get; set; }

    int SaveChanges();
    Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
      CancellationToken cancellationToken = default(CancellationToken));

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
  }

}
