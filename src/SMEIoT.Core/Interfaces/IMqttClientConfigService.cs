using System;
using System.Threading.Tasks;
using SMEIoT.Core.Entities;

namespace SMEIoT.Core.Interfaces
{
  public interface IMqttClientConfigService
  {
    string GetHost();
    int GetPort();
    Task<MqttBrokerConnectionInformation> SuggestConfigAsync();
  }
}
