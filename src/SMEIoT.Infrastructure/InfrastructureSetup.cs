using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NodaTime;
using System;
using SMEIoT.Core.Interfaces;
using SMEIoT.Infrastructure.Data;
using Hangfire;
using Hangfire.LiteDB;
using SMEIoT.Infrastructure.MqttClient;
using SMEIoT.Core.EventHandlers;
using SMEIoT.Core.Services;
using SMEIoT.Infrastructure.Services;
using Npgsql;
using Microsoft.Extensions.FileProviders;

namespace SMEIoT.Infrastructure
{
  public static class InfrastructureSetup
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

    public static void ConfigureHangfire(IGlobalConfiguration configuration)
    {
      configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        // we want less dependencies.
        // and hangfire PostgreSql can't store Nodatime.
        // Cache size 5000 * 4KB (page) = 20M memory
        .UseLiteDbStorage($"Filename=Hangfire.db; Mode=Shared; Cache Size=5000; Flush=true");
    }

    public static void ConfigureMqttClient(this IServiceCollection services, IConfiguration configuration)
    {
      services.AddSingleton<IMosquittoMessageHandler, MosquittoMessageHandler>();
      services.AddTransient<IMosquittoClientService, MosquittoClientService>();

      if (configuration.GetSection("SMEIoT")?.GetValue<bool>("UseMosquittoBackgroundClient") == true) {
        services.AddHostedService<BackgroundMqttClientHostedService>();
      }
    }

    public static void AddInfrastructureServices(this IServiceCollection services, IHostEnvironment env, IConfiguration configuration)
    {
      services.AddSingleton<IClock>(SystemClock.Instance);
      if (env.IsProduction() || env.IsStaging())
      {
        services.AddLetsEncrypt();
      }
      services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
      services.AddSingleton<IMqttIdentifierService, MqttIdentifierService>();
      services.AddSingleton<IMosquittoBrokerPidAccessor, MosquittoBrokerPidAccessor>();
      services.AddSingleton<IMosquittoBrokerPluginPidService, MosquittoBrokerPluginPidService>();
      services.AddSingleton<IMosquittoBrokerService, MosquittoBrokerService>();
      services.AddSingleton<IMosquittoClientAuthenticationService, MosquittoClientAuthenticationService>();
      services.AddTransient<IMosquittoBrokerMessageService, MosquittoBrokerMessageService>();
      services.AddSingleton<IMqttMessageDispatchService, MqttMessageDispatchService>();
      services.AddTransient<IMqttClientConfigService, MqttClientConfigService>();
      services.AddScoped<IMqttMessageIngestionService, MqttMessageIngestionService>();
      services.AddTransient<IIdentifierDictionaryFileAccessor, IdentifierDictionaryFileAccessor>(provider => {
        return new IdentifierDictionaryFileAccessor(env.ContentRootFileProvider);
      });
      services.AddTransient<ISystemSystemOneLineFileAccessor, SystemOneLineFileAccessor>(provider => {
        var fileProvider = new PhysicalFileProvider(configuration.GetSection("SMEIoT")?.GetValue<string>("SystemFilesRoot"));
        return new SystemOneLineFileAccessor(fileProvider);
      });
      services.AddTransient<IServerNetworkInterfacesIpAccessor, ServerNetworkInterfacesIpAccessor>();
    }
  }
}
