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
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
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
      services.AddSingleton<MosquittoClientAuthenticationService>();
      services.AddSingleton<MosquittoMessageHandler>();

      services.AddHostedService<BackgroundMqttClientHostedService>(provider =>
      {
        var auth = provider.GetService<MosquittoClientAuthenticationService>();

        var builder = new MosquittoClientBuilder()
          .SetPskTls(auth.ClientPsk, auth.ClientName)
          .SetConnectionInfo(configuration.GetConnectionString("MqttHost"), int.Parse(configuration.GetConnectionString("MqttPort")))
          .SetKeepAlive(60)
          .SetRunLoopInfo(-1, 1, 10)
          .SubscribeTopic(MosquittoClientBuilder.BrokerTopic)
          .SubscribeTopic(MosquittoClientBuilder.SensorTopic);

        var handler = provider.GetService<MosquittoMessageHandler>();
        builder.SetMessageCallback(handler.HandleMessage);
        return new BackgroundMqttClientHostedService(builder.Client);
      });
    }

    public static void AddInfrastructureServices(this IServiceCollection services, IHostEnvironment env)
    {
      services.AddSingleton<IClock>(SystemClock.Instance);
      services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
      services.AddSingleton<IMqttIdentifierService, MqttIdentifierService>();
      services.AddSingleton<IMosquittoBrokerService, MosquittoBrokerService>();
      services.AddSingleton<IFileProvider>(provider => {
        return env.ContentRootFileProvider;
      });
      services.AddScoped<IIdentifierDictionaryFileAccessor, IdentifierDictionaryFileAccessor>();
      services.AddScoped<IDeviceSensorIdentifierSuggestService, DeviceSensorIdentifierSuggestService>();
      // services.AddScoped<IPreSharedKeyGenerator, PreSharedKeyGenerator>();
    }
  }
}
