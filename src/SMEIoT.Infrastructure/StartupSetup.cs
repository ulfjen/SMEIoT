using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using NodaTime;
using SMEIoT.Core.Interfaces;
using SMEIoT.Infrastructure.Data;
using Hangfire;
using SMEIoT.Infrastructure.MqttClient;
using SMEIoT.Core.EventHandlers;
using Hangfire.LiteDB;
using SMEIoT.Core.Services;
using Npgsql;
using System.Data;

namespace SMEIoT.Infrastructure
{
  public static class StartupSetup
  {
    public static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
      services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(configuration.BuildConnectionString(), opts => opts.UseNodaTime()));
      NpgsqlConnection.GlobalTypeMapper.UseNodaTime();
      services.AddTransient<IApplicationDbConnection>(provider =>
      {
        return new ApplicationDbConnection(new NpgsqlConnection(configuration.BuildConnectionString()));
      });
    }

    public static void ConfigureHangfire(this IServiceCollection services, IConfiguration configuration) =>
      Hangfire.GlobalConfiguration.Configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseLiteDbStorage("Filename=Hangfire.db; Mode=Exclusive");
        // we want less dependencies.
        // and hangfire PostgreSql can't store Nodatime.
        // .UsePostgreSqlStorage(configuration.BuildConnectionString());
        // .UseRedisStorage(Redis);

    public static void ConfigureMqttClient(this IServiceCollection services, IConfiguration configuration)
    {
      services.AddSingleton<MosquittoMessageHandler>();

      var builder = new MosquittoClientBuilder()
        .SetConnectionInfo(configuration.GetConnectionString("MqttHost"), 8884)
        .SetKeepAlive(60)
        .SetPskTls(configuration.GetConnectionString("MqttPsk"), configuration.GetConnectionString("MqttIdentity"))
        .SetRunLoopInfo(-1, 1, 10)
        .SubscribeTopic(MosquittoClientBuilder.BrokerTopic)
        .SubscribeTopic(MosquittoClientBuilder.SensorTopic);

      services.AddHostedService<BackgroundMqttClientHostedService>(provider =>
      {
        var handler = provider.GetService<MosquittoMessageHandler>();
        builder.SetMessageCallback(handler.HandleMessage);
        return new BackgroundMqttClientHostedService(builder.Client);
      });
    }

    public static void AddInfrastructure(this IServiceCollection services)
    {
      services.AddSingleton<IClock>(SystemClock.Instance);
      services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
      services.AddSingleton<IMqttIdentifierService, MqttIdentifierService>();
      services.AddScoped<IdentifierDictionaryFileAccessor>();
      services.AddScoped<IDeviceSensorIdentifierSuggestService, DeviceSensorIdentifierSuggestService>();
      // services.AddScoped<IPreSharedKeyGenerator, PreSharedKeyGenerator>();
    }
  }
}
