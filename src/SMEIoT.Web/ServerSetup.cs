using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Configuration;
using SMEIoT.Infrastructure.Mosquitto;
using System.IO;
using System;

namespace SMEIoT.Web
{
  public static class ServerSetup
  {
    public static void ConfigureKestrel(WebHostBuilderContext context, KestrelServerOptions options)
    {
      var unixSocket = context.Configuration.GetSection("SMEIoT")?.GetValue<string>("MosquittoLocalAuthenticationSocket");
      if (unixSocket == null) {
        return;
      }

      if (File.Exists(unixSocket))
      {
        File.Delete(unixSocket);
      }
      options.ListenUnixSocket(unixSocket, builder =>
      {
        builder.Protocols = HttpProtocols.None;
        builder.UseConnectionHandler<MosquittoBrokerAuthHandler>();
      });
    }
  }
}
