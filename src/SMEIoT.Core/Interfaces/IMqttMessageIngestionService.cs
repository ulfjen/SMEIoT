using System.Threading.Tasks;
using SMEIoT.Core.Entities;

namespace SMEIoT.Core.Interfaces
{
  public interface IMqttMessageIngestionService
  {
    // store sensor value if registered
    // register device/sensor name for identification
    // update last message timestamps
    Task ProcessCommonMessageAsync(MqttMessage message);

    // stores broker values
    Task ProcessBrokerMessageAsync(MqttMessage message);
  }
}
