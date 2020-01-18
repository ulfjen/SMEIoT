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
using Microsoft.AspNetCore.TestHost;
using Moq;

namespace SMEIoT.IntegrationTests
{
  public class SeedWebApplicationFactory<TStartup> 
        : WebApplicationFactory<TStartup> where TStartup: class
  {
    protected override IWebHostBuilder CreateWebHostBuilder()
    {
      return Program.CreateHostBuilder(new string[]{});
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
      base.ConfigureWebHost(builder);
      builder.UseEnvironment("Test")
        .UseSolutionRelativeContentRoot("src/SMEIoT.Web"); 
    }
  }
}
