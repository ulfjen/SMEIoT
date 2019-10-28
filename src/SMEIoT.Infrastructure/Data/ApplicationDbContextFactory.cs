using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using NodaTime;

namespace SMEIoT.Infrastructure.Data
{
  public class ApplicationDbContextFactory: IDesignTimeDbContextFactory<ApplicationDbContext>
  {
    public ApplicationDbContext CreateDbContext(string[] args)
    {
      // Get environment
      string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

      // Build config
      var dir = Path.Combine(Directory.GetCurrentDirectory(), "..", "SMEIoT.Web");
      Console.WriteLine($"Config root: {dir}");
      IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(dir)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{environment}.json", optional: true)
        .AddUserSecrets("aspnet-SMEIoT-E793A15C-2A48-412E-A9B8-87778666BCC1")
        .AddEnvironmentVariables()
        .Build();
      
      var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
      optionsBuilder.UseNpgsql(config.BuildConnectionString(), opts => opts.UseNodaTime());

      return new ApplicationDbContext(optionsBuilder.Options, SystemClock.Instance);
    }
  }
}
