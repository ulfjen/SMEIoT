using SMEIoT.Core.Interfaces;

namespace SMEIoT.Infrastructure.Data
{
  public class MosquittoBrokerPidAccessor : IMosquittoBrokerPidAccessor
  {
    private readonly IOneLineFileAccessor _accessor;

    public MosquittoBrokerPidAccessor(IOneLineFileAccessor accessor)
    {
      _accessor = accessor;
    }

    public int? BrokerPid => GetBrokerPidFromPidFile("/var/run/smeiot.mosquitto.pid");

    public int? GetBrokerPidFromPidFile(string path)
    {
      var txt = _accessor.GetLine(path);
      if (txt == null) { return null; }
      if (int.TryParse(txt, out var parsed))
      {
        return parsed;
      }
      return null;
    }
  }
}
