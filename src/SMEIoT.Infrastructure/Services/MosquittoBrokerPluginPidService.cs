using SMEIoT.Core.Interfaces;

namespace SMEIoT.Infrastructure.Services
{
  public class MosquittoBrokerPluginPidService : IMosquittoBrokerPluginPidService
  {
    public int? BrokerPidFromAuthPlugin { get; set; }
  }
}
