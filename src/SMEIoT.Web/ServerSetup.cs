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
      var unixSocket = "/var/SMEIoT/run/smeiot.auth.broker";
      if (File.Exists(unixSocket))
      {
        File.Delete(unixSocket);
      }
      // TODO: don't use this address in dev. might be tricky to config
      options.ListenUnixSocket(unixSocket, builder =>
      {
        builder.Protocols = HttpProtocols.None;
        builder.UseConnectionHandler<MosquittoBrokerAuthHandler>();
      });
    }
  }
}
