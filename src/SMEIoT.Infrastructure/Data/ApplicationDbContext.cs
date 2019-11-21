using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NodaTime;
using SMEIoT.Core;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Interfaces;

namespace SMEIoT.Infrastructure.Data
{
  /// <summary>
  /// 
  /// </summary>
  /// <para>Per day, with 30s interval, a sensor generates 2880 data points.
  /// For a month, it generates 86400 data points. With 100 sensors, 8,640,000 data points were generated.
  /// </para>
  /// https://www.postgresql.org/docs/9.1/ddl-partitioning.html
  public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<long>, long, IdentityUserClaim<long>,
    IdentityUserRole<long>, IdentityUserLogin<long>, IdentityRoleClaim<long>, IdentityUserToken<long>>, IApplicationDbContext
  {
    private IClock _clock;
    // inherited Users, UserRoles DbSet
    public DbSet<Sensor> Sensors { get; set; } = null!;
    public DbSet<Device> Devices { get; set; } = null!;

    public DbSet<UserSensor> UserSensors { get; set; } = null!;

    // public override DbSet<IdentityUserRole<long>> UserRoles { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IClock clock)
      : base(options)
    {
      _clock = clock;
    }

    private void ApplyApplicationDbContextConcepts()
    {
      var entries = ChangeTracker.Entries();
      foreach (var entry in entries)
      {
        switch (entry.State)
        {
          case EntityState.Added:
            SetCreationAuditProperties(entry.Entity);
            break;
          case EntityState.Modified:
            SetModificationAuditProperties(entry);
            break;
          case EntityState.Deleted:
            break;
          case EntityState.Detached:
            break;
          case EntityState.Unchanged:
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }
    }

    protected virtual void SetCreationAuditProperties(object entityAsObj)
    {
      if (!(entityAsObj is IAuditTimestamp entityTimestamp))
      {
        return;
      }

      entityTimestamp.CreatedAt = entityTimestamp.UpdatedAt = _clock.GetCurrentInstant();
    }

    protected virtual void SetModificationAuditProperties(object entityAsObj)
    {
      if (!(entityAsObj is IAuditTimestamp entityTimestamp))
      {
        return;
      }

      entityTimestamp.UpdatedAt = _clock.GetCurrentInstant();
    }


    public override int SaveChanges()
    {
      ApplyApplicationDbContextConcepts();
      return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ApplyApplicationDbContextConcepts();
      return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);
      modelBuilder.UseSnakeCaseNames();
      modelBuilder.UseIdentityColumns();
      modelBuilder.Entity<User>().ToTable("users");
      modelBuilder.Entity<IdentityRole<long>>().ToTable("roles")
        .HasData(
          new IdentityRole<long> {Id = 1, Name = "Admin", NormalizedName = "Admin".ToUpper()});
      modelBuilder.Entity<IdentityUserRole<long>>().ToTable("user_roles");
      modelBuilder.Entity<IdentityUserClaim<long>>().ToTable("user_claims");
      modelBuilder.Entity<IdentityUserLogin<long>>().ToTable("user_logins");
      modelBuilder.Entity<IdentityRoleClaim<long>>().ToTable("role_claims");
      modelBuilder.Entity<IdentityUserToken<long>>().ToTable("user_tokens");

      modelBuilder.Entity<UserSensor>()
        .HasKey(us => new {us.UserId, us.SensorId});

      modelBuilder.Entity<UserSensor>()
        .HasOne(us => us.User)
        .WithMany(u => u.UserSensors)
        .HasForeignKey(us => us.SensorId);

      modelBuilder.Entity<UserSensor>()
        .HasOne(us => us.Sensor)
        .WithMany(s => s.UserSensors)
        .HasForeignKey(us => us.SensorId);

      modelBuilder.Entity<Device>()
        .Property(d => d.AuthenticationType)
        .HasConversion(
            v => v.ToString(),
            v => (DeviceAuthenticationType)Enum.Parse(typeof(DeviceAuthenticationType), v));
    }
  }
}
