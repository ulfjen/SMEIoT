using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Logging;
using Hangfire;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using SMEIoT.Core.Entities;
using SMEIoT.Infrastructure;
using SMEIoT.Infrastructure.Data;
using SMEIoT.Core.Interfaces;
using SMEIoT.Core.Services;
using SMEIoT.Web.Api.Filters;
using SMEIoT.Web.Hubs;
using SMEIoT.Web.Api.Config;
using SMEIoT.Web.Services;
using SMEIoT.Web.Middlewares;

namespace SMEIoT.Web
{
  public class Startup
  {
    private IWebHostEnvironment _env;

    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
      Configuration = configuration;
      _env = env;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddDbContext(Configuration);

      services.AddInfrastructureServices(_env);
      services.ConfigureHangfire(Configuration);
      services.AddHangfire(globalConfig => { });

      services.AddHangfireServer(options =>
      {
        options.Queues = new[] { "critical", "default" };
      });

      services.AddTransient<ProblemDetailsFactory, SMEIoTProblemDetailsFactory>();
      services.AddScoped<ISensorAssignmentService, SensorAssignmentService>();
      services.AddScoped<IUserManagementService, UserManagementService>();
      services.AddScoped<IUserProfileService, UserProfileService>();
      services.AddScoped<IDeviceService, DeviceService>();
      services.AddScoped<ISensorValueService, SensorValueService>();
      services.AddTransient<ISecureKeySuggestionService, SecureKeySuggestionService>();

      services.ConfigureMqttClient(Configuration);

      StartupConfigureIdentity.Configure<User, IdentityRole<long>, ApplicationDbContext>(services);

      services.AddVersionedApiExplorer(options => options.GroupNameFormat = "'v'VVV");
      services.AddApiVersioning(options =>
      {
        options.ReportApiVersions = true;
        options.ApiVersionReader = new MediaTypeApiVersionReader();
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ApiVersionSelector = new CurrentImplementationApiVersionSelector(options);
        options.DefaultApiVersion = new ApiVersion(new DateTime(2019, 09, 27), 1, 0);
        options.Conventions.Add(new VersionByNamespaceConvention());
      });
      services.AddScoped<LastSeenFilter>();
      services.AddControllersWithViews(options => {
        options.Filters.Add(typeof(ActionExceptionFilter));
        options.Filters.Add(typeof(LastSeenFilter));
        })
        .AddJsonOptions(options =>
        {
          options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
          options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
          options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
          options.JsonSerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
        });
      services.AddRouting(options =>
      {
        options.LowercaseUrls = true;
        options.ConstraintMap["slugify"] = typeof(SlugifyParameterTransformer);
      });
      services.AddAntiforgery(options => {
        options.HeaderName = "X-CSRF-TOKEN";
        options.Cookie.Name = "X-CSRF-TOKEN-COOKIE";
      });

      if (_env.IsDevelopment()) {
        services.AddSwaggerGen(c =>
        {
          c.OperationFilter<ApiVersionOperationFilter>();
          c.SchemaFilter<RequiredSchemaFilter>();
          c.SwaggerDoc("v1", new OpenApiInfo { Title = "SMEIoT API", Version = "v1" });
        });
      }
      services.AddRazorPages();
      services.AddSignalR();
      services.AddSingleton<MqttMessageHubDeliveryService>(provider =>
      {
        var deliveryService = new MqttMessageHubDeliveryService(
          provider.GetRequiredService<Microsoft.AspNetCore.SignalR.IHubContext<MqttHub>>()
        );
        var handler = provider.GetService<Core.EventHandlers.MosquittoMessageHandler>();
        handler.Attach(deliveryService);
        return deliveryService;
      });

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory,
      IServiceProvider serviceProvider)
    {
      if (env.IsDevelopment())
      {
        app.UseMiddleware<DevThrottleMiddleware>();

        app.UseDeveloperExceptionPage();
        app.UseDatabaseErrorPage();
        app.UseHangfireDashboard();
        app.UseSwagger(c => {
          c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
          {
            if (httpReq.Host.Host == "localhost") {
              swaggerDoc.Servers = new List<OpenApiServer> {
                new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" },
                new OpenApiServer { Url = "https://smeiot.se" }
              };
            } else {
              swaggerDoc.Servers = new List<OpenApiServer> {
                new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" }
              };
            }
          });
        });

        app.UseSwaggerUI(c =>
        {
          var swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
          c.SwaggerEndpoint("/swagger/v1/swagger.json", "SMEIoT API V1");
        });

      }
      else
      {
        app.UseExceptionHandler("/error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      Hangfire.GlobalConfiguration.Configuration
        .UseActivator(new HangfireActivator(serviceProvider));
      app.UseHttpsRedirection();

      app.UseStaticFiles(new StaticFileOptions
      {
        OnPrepareResponse = ctx =>
        {
          const int durationInSeconds = 60 * 60 * 24;
          ctx.Context.Response.Headers[HeaderNames.CacheControl] =
            $"public,max-age={durationInSeconds}";
        }
      });

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapRazorPages();
        endpoints.MapControllerRoute("create_login", "/login", new { controller = "Sessions", action = "Create" });
        endpoints.MapControllerRoute("destroy_login", "/logout", new { controller = "Sessions", action = "Destroy" },
          new { httpMethod = new HttpMethodRouteConstraint(nameof(HttpMethod.Delete)) });

        endpoints.MapControllers();
        endpoints.MapHub<MqttHub>("/dashboard/mqtt_hub");
      });

      app.UseSpa(spa =>
      {
        spa.Options.SourcePath = "../ClientApp";

        if (env.IsDevelopment())
        {
          spa.UseProxyToSpaDevelopmentServer("http://localhost:3000");
        }
      });
    }
  }
}
