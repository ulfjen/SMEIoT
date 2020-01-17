using System.Threading.Tasks;
using SMEIoT.Core.Entities;

namespace SMEIoT.Core.Interfaces
{
  // a singleton service to process messages
  public interface IMqttMessageIngestionService
  {
    // update last message timestamps
    // store sensor value if registered
    Task ProcessAsync(MqttMessage message);
  }
}
