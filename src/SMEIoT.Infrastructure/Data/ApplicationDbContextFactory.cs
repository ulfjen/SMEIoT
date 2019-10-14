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
      IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../SMEIoT.Web"))
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{environment}.json", optional: true)
        .AddEnvironmentVariables()
        .Build();
      
      var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
      optionsBuilder.UseNpgsql(config.BuildConnectionString(), opts => opts.UseNodaTime());

      return new ApplicationDbContext(optionsBuilder.Options, SystemClock.Instance);
    }
  }
}
