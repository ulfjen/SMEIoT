using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Net.Http.Headers;
using SMEIoT.Core.Entities;
using SMEIoT.Infrastructure;
using SMEIoT.Infrastructure.Data;
using Microsoft.OpenApi.Models;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using SMEIoT.Core.Interfaces;
using SMEIoT.Core.Services;
using SMEIoT.Web.Api.Filters;
using Hangfire;
using Microsoft.Extensions.Logging;
using SMEIoT.Web.Hubs;
using SMEIoT.Web.Services;

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

      services.AddScoped<ISensorAssignmentService, SensorAssignmentService>();
      services.AddScoped<IUserManagementService, UserManagementService>();
      services.AddScoped<IUserProfileService, UserProfileService>();
      services.AddScoped<IDeviceService, DeviceService>();
      services.AddTransient<ISecureKeySuggestService, SecureKeySuggestService>();

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
        options.Filters.Add(new ActionExceptionFilter());
        options.Filters.Add(typeof(LastSeenFilter));
        })
        .AddJsonOptions(options =>
        {
          options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
          options.JsonSerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
        });
      services.AddRouting(options =>
      {
        options.LowercaseUrls = true;
        options.ConstraintMap["slugify"] = typeof(SlugifyParameterTransformer);
      });

      services.AddSwaggerGen(c =>
      {
        c.OperationFilter<SwaggerDefaultValues>();
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "SMEIoT API", Version = "v1" });
      });
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
        app.UseDeveloperExceptionPage();
        app.UseDatabaseErrorPage();
      }
      else
      {
        app.UseExceptionHandler("/error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
        app.UseSpaStaticFiles();
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

      app.UseSwagger();

      app.UseSwaggerUI(c =>
      {
        var swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SMEIoT API V1");
      });

      app.UseCookiePolicy();
      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
        // endpoints.MapControllerRoute("new_login", "/login", new { controller = "Sessions", action = "New" },
        //   new { httpMethod = new HttpMethodRouteConstraint(nameof(HttpMethod.Get)) });
        endpoints.MapControllerRoute("create_login", "/login", new { controller = "Sessions", action = "Create" });
        endpoints.MapControllerRoute("destroy_login", "/logout", new { controller = "Sessions", action = "Destroy" },
          new { httpMethod = new HttpMethodRouteConstraint(nameof(HttpMethod.Delete)) });
        // endpoints.MapControllerRoute("signup", "/signup", new { controller = "Users", action = "New" });
        // endpoints.MapControllerRoute("edit_user", "/account", new { controller = "Users", action = "Edit" });

        // endpoints.MapControllerRoute("dashboard", "/dashboard/{*url}", new { controller = "Dashboard", action = "Index" });
        // endpoints.MapControllerRoute("app", "/{*url}", new { controller = "Home", action = "Index" });

        // endpoints.MapControllerRoute(
        //   name: "default",
        //   pattern: "{controller:slugify=Home}/{action:slugify=Index}/{id?}");

        endpoints.MapRazorPages();
        endpoints.MapHub<MqttHub>("/dashboard/mqtt_hub");
      });

      app.UseSpa(spa =>
      {
        spa.Options.SourcePath = "ClientApp";

        if (env.IsDevelopment())
        {
          spa.UseProxyToSpaDevelopmentServer("http://localhost:3000");
        }
      });
    }
  }
}
