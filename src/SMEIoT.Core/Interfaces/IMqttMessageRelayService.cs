using System.Threading.Tasks;
using SMEIoT.Core.Entities;

namespace SMEIoT.Core.Interfaces
{
  // a singleton service to relay messages
  public interface IMqttMessageRelayService
  {
    Task ProcessAsync(MqttMessage message);
  }
}
