using System;
using System.Collections.Generic;
using System.Linq;
using static SMEIoT.Infrastructure.MqttClient.MosquittoWrapper;

namespace SMEIoT.Infrastructure.MqttClient
{
  /// <summary>
  /// must be used as a singleton otherwise it's not thread safe.
  /// The parameters are needed to be configured properly because the error doesn't surface up to CLR.
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
      var res = MosquittoWrapper.mosq_set_tls_psk(Psk, Identity, Ciphers);
      if (res != 0)
      {
        throw new ArgumentException($"Mosquitto mosq_set_tls_psk returned {res}");
      }
      MosquittoWrapper.mosq_set_callback(ConnectCallback, MessageCallback);

      if (Host == null)
      {
        throw new ArgumentException($"Mosquitto host is null");
      }

      res = MosquittoWrapper.mosq_connect(Host!, Port, KeepAlive);
      if (res != 0)
      {
        throw new ArgumentException($"Mosquitto mosq_connect returned {res}");
      }

      var res_list = Topics.Select(t => MosquittoWrapper.mosq_subscribe_topic(t));
      if (res_list.Any(r => r != 0))
      {
        throw new ArgumentException("Mosquitto mosq_subscribe_topic returned error", string.Join(',', res_list));
      }
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
