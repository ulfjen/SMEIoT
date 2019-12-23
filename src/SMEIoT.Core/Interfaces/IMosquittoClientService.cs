using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NodaTime;

namespace SMEIoT.Core.Interfaces
{
  public interface IMosquittoClientService : IDisposable
  {
    string? Host { get; }
    int Port { get; }
    int KeepAlive { get; }
    string Psk { get; }
    string Identity { get; }
    string? Ciphers { get; }
    List<string> Topics { get; }
    int Timeout { get; }
    int MaxPackets { get; }
    int SleepOnReconnect { get; }

    void SubscribeTopic(string topic);
    void Connect();
    int RunLoop();
    int Reconnect();
  }
}
