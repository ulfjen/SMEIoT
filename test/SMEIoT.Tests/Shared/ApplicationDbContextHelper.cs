using System;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using NodaTime.Testing;
using SMEIoT.Infrastructure.Data;
 
namespace SMEIoT.Tests.Shared
{
  public static class ApplicationDbContextHelper
  {
    public static ApplicationDbContext BuildTestDbContext(string? databaseName = null, Instant? initial = null)
    {
      if (databaseName == null)
      {
        databaseName = Guid.NewGuid().ToString();
      }

      if (initial == null)
      {
        initial = SystemClock.Instance.GetCurrentInstant();
      }

      var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(databaseName)
        .EnableSensitiveDataLogging()
        .Options;

      return new ApplicationDbContext(options, new FakeClock(initial.Value));
    }
  }
}
