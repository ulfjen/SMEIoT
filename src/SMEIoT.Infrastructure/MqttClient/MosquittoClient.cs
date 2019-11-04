using System;
using System.Collections.Generic;
using System.Linq;
using static SMEIoT.Infrastructure.MqttClient.MosquittoWrapper;

namespace SMEIoT.Infrastructure.MqttClient
{
  /// <summary>
  /// must be used as a singleton otherwise it's not thread safe.
  /// </summary>
  public class MosquittoClient : IDisposable
  {
    public string? Host { get; internal set; }
    public int Port { get; internal set; }
    public int KeepAlive { get; internal set; }
    public string Psk { get; internal set; }
    public string Identity { get; internal set; }
    public string? Ciphers { get; internal set; }
    public IEnumerable<string> Topics { get; internal set; }
    public int Timeout { get; internal set; }
    public int MaxPackets { get; internal set; }
    public int SleepOnReconnect { get; internal set; }

    public ConnectCallbackDelegate? ConnectCallback { get; internal set; }
    public MessageCallbackDelegate? MessageCallback { get; internal set; }

    public MosquittoClient()
    {
      MosquittoWrapper.mosq_init();   
    }

    public void Connect()
    {
      MosquittoWrapper.mosq_set_tls_psk(Psk, Identity, Ciphers);
      MosquittoWrapper.mosq_set_callback(ConnectCallback, MessageCallback);
      MosquittoWrapper.mosq_connect(Host, Port, KeepAlive);
      Topics.Select(t => MosquittoWrapper.mosq_subscribe_topic(t));
    }

    public void RunLoop()
    {
      MosquittoWrapper.mosq_runloop(Timeout, MaxPackets, SleepOnReconnect);
    }

    public void Dispose()
    {
      MosquittoWrapper.mosq_destroy();
    }
  }
}
