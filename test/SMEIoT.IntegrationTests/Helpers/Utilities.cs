using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using SMEIoT.Core.Entities;
using SMEIoT.Infrastructure.Data;

namespace SMEIoT.IntegrationsTests.Helpers
{
  public static class Utilities
  {
    public static async void InitializeDbForTests(ApplicationDbContext db, UserManager<User> userManager)
    {
      await userManager.CreateAsync(new User {UserName = "admin", SecurityStamp = Guid.NewGuid().ToString()}, "a-normal-password-123");
      await userManager.CreateAsync(new User {UserName = "normal-user-1", SecurityStamp = Guid.NewGuid().ToString()}, "another-password-1");
      await userManager.CreateAsync(new User {UserName = "normal-user-2", SecurityStamp = Guid.NewGuid().ToString()}, "another-password-2");
      await db.Sensors.AddAsync(new Sensor
      {
        Name = "a-normal-sensor", NormalizedName = Sensor.NormalizeName("a-normal-sensor")
      });
      db.SaveChanges();
    }

    public static async void ReinitializeDbForTests(ApplicationDbContext db, UserManager<User> userManager)
    {
      foreach (var userName in new[] {"admin", "normal-user-1", "normal-user-2"})
      {
        await userManager.DeleteAsync(await userManager.FindByNameAsync(userName));
      }
      db.Sensors.RemoveRange(db.Sensors.Where(s => s.NormalizedName == Sensor.NormalizeName("a-normal-sensor")));
      InitializeDbForTests(db, userManager);
    }
  }

}
