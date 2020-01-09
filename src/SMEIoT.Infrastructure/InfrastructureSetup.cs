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
using SMEIoT.Infrastructure.Services;
using Npgsql;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Data;

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

    public static void ConfigureHangfire(this IServiceCollection services, IConfiguration configuration)
    {
      Hangfire.GlobalConfiguration.Configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        // we want less dependencies.
        // and hangfire PostgreSql can't store Nodatime.
        .UseLiteDbStorage($"Filename=Hangfire.db; Mode=Shared; Cache Size=5000");
    }

    public static void ConfigureMqttClient(this IServiceCollection services, IConfiguration configuration)
    {
      services.AddSingleton<MosquittoMessageHandler>();
      services.AddTransient<IMosquittoClientService, MosquittoClientService>();

      if (configuration.GetSection("SMEIoT").GetValue<bool>("UseMosquittoBackgroundClient")) {
        services.AddHostedService<BackgroundMqttClientHostedService>();
      }
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
      services.AddScoped<IMqttEntityIdentifierSuggestionService, MqttEntityIdentifierSuggestionService>();
    }
  }
}
