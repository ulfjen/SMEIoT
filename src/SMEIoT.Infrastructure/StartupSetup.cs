using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using NodaTime;
using System;
using SMEIoT.Core.Interfaces;
using SMEIoT.Infrastructure.Data;
using Hangfire;
using Hangfire.LiteDB;
using SMEIoT.Infrastructure.MqttClient;
using SMEIoT.Core.EventHandlers;
using SMEIoT.Core.Services;
using Npgsql;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Data;

namespace SMEIoT.Infrastructure
{
  public static class StartupSetup
  {
    public static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
      services.AddDbContext<ApplicationDbContext>(options => {
        options.UseNpgsql(configuration.BuildConnectionString(), optionsBuilder => optionsBuilder.UseNodaTime());
      });

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
        // we want less dependencies.
        // and hangfire PostgreSql can't store Nodatime.
        .UseLiteDbStorage("Filename=Hangfire.db; Mode=Exclusive");

    public static void ConfigureMqttClient(this IServiceCollection services, IConfiguration configuration)
    {
      services.AddSingleton<MosquittoMessageHandler>();

      services.AddHostedService<BackgroundMqttClientHostedService>(provider =>
      {
        var auth = provider.GetService<IMosquittoClientAuthenticationService>();
        var broker = provider.GetService<IMosquittoBrokerService>();
        var env = provider.GetService<IHostEnvironment>();
        int port;
        var portStr = configuration.GetConnectionString("MqttPort");
        if (!int.TryParse(portStr, out port)) {
          throw new InvalidOperationException($"MqttPort is not set to a correct value. Got {portStr} but expect a number");
        }

        var builder = new MosquittoClientBuilder()
          .SetPskTls(auth.ClientPsk, auth.ClientName)
          .SetConnectionInfo(configuration.GetConnectionString("MqttHost"), port)
          .SetKeepAlive(60)
          .SetRunLoopInfo(-1, 10)
          .SubscribeTopic(MosquittoClientBuilder.BrokerTopic)
          .SubscribeTopic(MosquittoClientBuilder.SensorTopic);

        var handler = provider.GetService<MosquittoMessageHandler>();
        builder.SetMessageCallback(handler.HandleMessage);
        return new BackgroundMqttClientHostedService(builder.Client, provider.GetService<ILogger<BackgroundMqttClientHostedService>>(), broker, env);
      });
    }

    public static void AddInfrastructureServices(this IServiceCollection services, IHostEnvironment env)
    {
      services.AddSingleton<IClock>(SystemClock.Instance);
      if (env.IsProduction() || env.IsStaging())
      {
        services.AddLetsEncrypt();
      }
      services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
      services.AddSingleton<IMqttIdentifierService, MqttIdentifierService>();
      services.AddSingleton<IMosquittoBrokerService, MosquittoBrokerService>();
      services.AddSingleton<IMosquittoClientAuthenticationService, MosquittoClientAuthenticationService>();
      services.AddTransient<IMosquittoBrokerMessageService, MosquittoBrokerMessageService>();
      services.AddSingleton<IFileProvider>(provider => {
        return env.ContentRootFileProvider;
      });
      services.AddScoped<IIdentifierDictionaryFileAccessor, IdentifierDictionaryFileAccessor>();
      services.AddScoped<IDeviceSensorIdentifierSuggestService, DeviceSensorIdentifierSuggestService>();
    }
  }
}
