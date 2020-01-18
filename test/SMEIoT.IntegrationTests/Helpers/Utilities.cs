using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SMEIoT.Core.Entities;
using SMEIoT.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace SMEIoT.IntegrationsTests.Helpers
{
  public static class Utilities
  {
    public static async Task InitializeDbForTests(ApplicationDbContext db, UserManager<User> userManager)
    {
      await userManager.CreateAsync(new User {UserName = "admin", SecurityStamp = Guid.NewGuid().ToString()}, "a-normal-password-123");
      await userManager.CreateAsync(new User {UserName = "normal-user-1", SecurityStamp = Guid.NewGuid().ToString()}, "another-password-1");
      await userManager.CreateAsync(new User {UserName = "normal-user-2", SecurityStamp = Guid.NewGuid().ToString()}, "another-password-2");
      var device = new Device
      {
        Name = "device-1", NormalizedName = Device.NormalizeName("device-1")
      };
      await db.Devices.AddAsync(device);
      await db.Sensors.AddAsync(new Sensor
      {
        Name = "sensor-1", NormalizedName = Sensor.NormalizeName("sensor-1"),
        Device = device
      });
      await db.SaveChangesAsync();
    }

    public static async Task ReinitializeDbForTests(ApplicationDbContext db, UserManager<User> userManager)
    {
      db.Database.ExecuteSqlInterpolated($"TRUNCATE user_roles, user_tokens, user_logins, user_claims, role_claims, roles, devices, user_sensors, sensors, users RESTART IDENTITY CASCADE;");
      await InitializeDbForTests(db, userManager);
    }
  }

}
