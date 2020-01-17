using System.Threading.Tasks;
using SMEIoT.Core.Entities;

namespace SMEIoT.Core.Interfaces
{
  // a singleton service to process messages and resolve scope
  public interface IMqttMessageDispatchService
  {
    Task ProcessAsync(MqttMessage message);
  }
}
