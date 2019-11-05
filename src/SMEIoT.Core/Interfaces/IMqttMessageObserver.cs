using System.Threading.Tasks;
using SMEIoT.Core.Entities;

namespace SMEIoT.Core.Interfaces
{
  public interface IMqttMessageObserver
  {
    void Update(MqttMessage message);
  }
}
