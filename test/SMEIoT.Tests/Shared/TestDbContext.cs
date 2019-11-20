using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Interfaces;

namespace SMEIoT.Tests.Shared
{
  public class TestDbContext: DbContext, IApplicationDbContext
  {
    public TestDbContext(DbContextOptions<TestDbContext> options)
      : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Sensor> Sensors { get; set; }
    public virtual DbSet<UserSensor> UserSensors { get; set; }
    public virtual DbSet<IdentityUserRole<long>> UserRoles { get; set; }
    public virtual DbSet<Device> Devices { get; set; }
  }
}
