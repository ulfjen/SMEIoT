using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using SMEIoT.Core.Entities;
using SMEIoT.Web;
using SMEIoT.Infrastructure;
using SMEIoT.Infrastructure.Data;
using SMEIoT.IntegrationsTests.Helpers;
using SMEIoT.Infrastructure.MqttClient;
using Npgsql;
using NodaTime;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace SMEIoT.IntegrationTests
{
  
  public class TestStartup : Startup
  {
    public TestStartup(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
    {
    }

    public override void ConfigureServices(IServiceCollection services)
    {
      base.ConfigureServices(services);
      var sp = services.BuildServiceProvider();
      
      // Create a scope to obtain a reference to the database
      // context (ApplicationDbContext).
      using (var scope = sp.CreateScope())
      {
        var scopedServices = scope.ServiceProvider;
        var db = scopedServices.GetService<ApplicationDbContext>();
        var logger = scopedServices
            .GetRequiredService<ILogger<TestStartup>>();

        // Ensure the database is created.
        db.Database.EnsureCreated();
        var userManager = scopedServices.GetRequiredService<UserManager<User>>();

        try
        {
          // Seed the database with test data.
          Utilities.InitializeDbForTests(db, userManager);
        }
        catch (Exception ex)
        {
          logger.LogError(ex, "An error occurred seeding the " +
              "database with test messages. Error: {Message}", ex.Message);
        }
      }
    }
  }
}
