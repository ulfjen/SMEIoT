using System;
using SMEIoT.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace SMEIoT.Infrastructure.Data
{
  public class MosquittoBrokerPidAccessor : IMosquittoBrokerPidAccessor
  {
    private readonly ISystemSystemOneLineFileAccessor _accessor;
    private readonly ILogger _logger;

    public MosquittoBrokerPidAccessor(
      ISystemSystemOneLineFileAccessor accessor,
      ILogger<MosquittoBrokerPidAccessor> logger)
    {
      _accessor = accessor;
      _logger = logger;
    }

    public int? BrokerPid => GetBrokerPidFromPidFile("smeiot.mosquitto.pid");

    public int? GetBrokerPidFromPidFile(string path)
    {
      var txt = _accessor.GetLine(path);
      _logger.LogTrace($"Got {txt} from {path}");
      if (txt == null) { return null; }
      if (int.TryParse(txt, out var parsed))
      {
        return parsed;
      }
      return null;
    }
  }
}
