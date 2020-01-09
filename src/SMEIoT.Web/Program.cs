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
      Thread.Sleep(5000);
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
// cd /tmp && sudo mkdir -p /tmp/smeiot_build && sudo tar xf /tmp/smeiot-config.tar.gz -C /tmp/smeiot_build
// bash -c 'source /tmp/smeiot_build/scripts/bootstrap.sh; build_smeiot_with_remote_tars'
