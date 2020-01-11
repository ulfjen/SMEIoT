using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NodaTime;
using NodaTime.Testing;
using SMEIoT.Infrastructure;
using SMEIoT.Infrastructure.Data;
 
namespace SMEIoT.Tests.Shared
{
  public static class ApplicationDbContextHelper
  {
    public static ApplicationDbContext BuildTestDbContext(IClock? clock = null)
    {
      if (clock == null)
      {
        clock = SystemClock.Instance;
      }
      
      var dir = Path.Combine(Directory.GetCurrentDirectory(), "..",　"..", "..");
      var config = new ConfigurationBuilder()
        .SetBasePath(dir)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.Test.json", optional: true)
        .AddUserSecrets("aspnet-SMEIoT-E793A15C-2A48-412E-A9B8-87778666BCC1")
        .AddEnvironmentVariables()
        .Build();

      var dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>();
      var dbContextOptions = dbOptions.UseNpgsql(config.BuildConnectionString(), optionsBuilder => optionsBuilder.UseNodaTime())
        .EnableSensitiveDataLogging()
        .Options;

      var context = new ApplicationDbContext(dbContextOptions, clock);

      return context;
    }
  }
}
