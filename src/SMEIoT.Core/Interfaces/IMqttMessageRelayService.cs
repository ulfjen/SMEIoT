using System.Threading.Tasks;
using SMEIoT.Core.Entities;

namespace SMEIoT.Core.Interfaces
{
  public interface IMqttMessageRelayService
  {
    Task RelayAsync(MqttMessage message);
  }
}
