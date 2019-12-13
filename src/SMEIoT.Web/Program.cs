using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace SMEIoT.Web
{
  public class Program
  {
    public static void Main(string[] args)
    {
      CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
          var env = hostingContext.HostingEnvironment;

          if (env.IsEnvironment("Test"))
          {
            var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
            config.AddUserSecrets(appAssembly, optional: true);
          }
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
          webBuilder.UseStartup<Startup>();
          webBuilder.ConfigureKestrel(ServerSetup.ConfigureKestrel);
        });
  }
}
