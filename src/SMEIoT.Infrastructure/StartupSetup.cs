using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using NodaTime;
using SMEIoT.Core.Interfaces;
using SMEIoT.Infrastructure.Data;
using Hangfire;
using StackExchange.Redis;
using System;
using SMEIoT.Infrastructure.MqttClient;
using System.Linq;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Client.Options;

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

    public static byte[] StringToByteArray(string hex)
    {
      return Enumerable.Range(0, hex.Length)
                       .Where(x => x % 2 == 0)
                       .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                       .ToArray();
    }

    public static void ConfigureMqttClient(this IServiceCollection services, IConfiguration configuration)
    {
      var mqttClientOptions = new ManagedMqttClientOptionsBuilder()
        .WithAutoReconnectDelay(TimeSpan.FromSeconds(5));
      var build = new MqttClientOptionsBuilder();
    build
        .WithClientId("Client1")
        .WithTcpServer("193.10.119.35")
        .WithTls(para =>
        {
          para.Certificates = configuration.GetConnectionString("MqttBrokerCertificates").Split(',').Select(StringToByteArray);
          para.AllowUntrustedCertificates = true;
          para.IgnoreCertificateChainErrors = true;
          para.IgnoreCertificateRevocationErrors = true;
        });

      mqttClientOptions.WithClientOptions(build.Build());

      string[] topicFilters = { "sensor/#" };
      services.AddHostedService<BackgroundMqttClientHostedService>(provider =>
      {
        return new BackgroundMqttClientHostedService(mqttClientOptions.Build(), topicFilters);
      });
    }

    public static void AddInfrastructure(this IServiceCollection services)
    {
      services.AddSingleton<IClock>(SystemClock.Instance);
      services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
    }
  }
}
