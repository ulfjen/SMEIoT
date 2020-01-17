using System;
using SMEIoT.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace SMEIoT.Core.Services
{
  public class MqttClientConfigService : IMqttClientConfigService
  {
    private readonly IConfiguration _config;

    public MqttClientConfigService(IConfiguration config)
    {
      _config = config;
    }

    public string GetHost()
    {
      var host = _config.GetConnectionString("MqttHost");
      if (string.IsNullOrEmpty(host))
      {
        throw new InvalidOperationException($"MqttHost is not set to a correct value. Got {host}.");
      }
      return host;
    }

    public int GetPort()
    {
      int port;
      var portStr = _config.GetConnectionString("MqttPort");
      if (!int.TryParse(portStr, out port)) {
        throw new InvalidOperationException($"MqttPort is not set to a correct value. Got {portStr} but expect a number");
      }
      return port;
    }
  }
}
