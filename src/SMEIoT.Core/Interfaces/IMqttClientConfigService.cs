using System;

namespace SMEIoT.Core.Interfaces
{
  public interface IMqttClientConfigService
  {
    string GetHost();
    int GetPort();
  }
}
