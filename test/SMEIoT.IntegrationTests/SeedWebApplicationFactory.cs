using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SMEIoT.Core.Entities;
using SMEIoT.Infrastructure.Data;
using SMEIoT.IntegrationsTests.Helpers;

namespace SMEIoT.IntegrationTests
{
  public class SeedWebApplicationFactory<TStartup> 
        : WebApplicationFactory<TStartup> where TStartup: class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
          base.ConfigureWebHost(builder);
            builder.ConfigureServices(services =>
            {
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database
                // context (ApplicationDbContext).
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<ApplicationDbContext>();
                    var logger = scopedServices
                        .GetRequiredService<ILogger<SeedWebApplicationFactory<TStartup>>>();

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
            });
        }
    }
}
