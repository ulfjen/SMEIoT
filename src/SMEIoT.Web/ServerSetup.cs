using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Connections;
using SMEIoT.Infrastructure.Mosquitto;
using System.IO;

namespace SMEIoT.Web
{
  public static class ServerSetup
  {
    public static void ConfigureKestrel(KestrelServerOptions options)
    {
      var unixSocket = "/tmp/smeiot.auth.broker";
      if (File.Exists(unixSocket))
      {
        // try
        File.Delete(unixSocket);
      }
      options.ListenUnixSocket(unixSocket, builder =>
      {
        builder.Protocols = HttpProtocols.None;
        builder.UseConnectionHandler<MosquittoBrokerAuthHandler>();
      });
      options.ListenLocalhost(5000);
      options.ListenLocalhost(5001, builder => {
        builder.UseHttps();
      });
    }
  }
}
