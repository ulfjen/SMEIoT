using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using NodaTime;
using SMEIoT.Core.Interfaces;
using SMEIoT.Infrastructure.Data;
using Hangfire;
using StackExchange.Redis;

namespace SMEIoT.Infrastructure
{
  public static class StartupSetup
  {
    public static void AddDbContext(this IServiceCollection services, IConfiguration configuration) =>
      services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(configuration.BuildConnectionString(), opts => opts.UseNodaTime()));

    public static ConnectionMultiplexer? Redis;

    public static void ConfigureRedis(this IServiceCollection services, IConfiguration configuration) =>
      Redis = ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis"));

    public static void ConfigureHangfire(this IServiceCollection services, IConfiguration configuration) =>
      Hangfire.GlobalConfiguration.Configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        // .UsePostgreSqlStorage(configuration.BuildConnectionString());
        .UseRedisStorage(Redis);

    public static void AddInfrastructure(this IServiceCollection services)
    {
      services.AddSingleton<IClock>(SystemClock.Instance);
      services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
    }
  }
}
