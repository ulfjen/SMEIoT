using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using SMEIoT.Infrastructure.Mosquitto;
using System.IO;
using System;

namespace SMEIoT.Web
{
  public static class ServerSetup
  {
    public static void ConfigureKestrel(WebHostBuilderContext context, KestrelServerOptions options)
    {
      var rootPath = context.Configuration.GetSection("SMEIoT")?.GetValue<string>("SystemFilesRoot");
      if (rootPath == null) {
        return;
      }
      var provider = new PhysicalFileProvider(rootPath);

      var unixSocketFileInfo = provider.GetFileInfo("smeiot.auth.broker");

      if (unixSocketFileInfo.Exists)
      {
        File.Delete(unixSocketFileInfo.PhysicalPath);
      }
      options.ListenUnixSocket(unixSocketFileInfo.PhysicalPath, builder =>
      {
        builder.Protocols = HttpProtocols.None;
        builder.UseConnectionHandler<MosquittoBrokerAuthHandler>();
      });
    }
  }
}
