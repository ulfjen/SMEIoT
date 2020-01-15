using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore;
using SMEIoT.Infrastructure;
using System.Threading;

namespace SMEIoT.Web
{
  public class Program
  {
    public static void Main(string[] args)
    {
      CreateHostBuilder(args).Build().Run();
    }

    public static IWebHostBuilder CreateHostBuilder(string[] args) =>
      WebHost.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
          var env = hostingContext.HostingEnvironment;

          if (env.IsEnvironment("Test"))
          {
            var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
            config.AddUserSecrets(appAssembly, optional: true);
          }
        })
        .UseStartup<Startup>()
        .ConfigureKestrel(ServerSetup.ConfigureKestrel);
  }
}
