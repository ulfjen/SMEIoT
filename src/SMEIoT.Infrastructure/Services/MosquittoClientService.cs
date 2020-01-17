using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using SMEIoT.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using SMEIoT.Core.EventHandlers;
using SMEIoT.Infrastructure.MqttClient;
using static SMEIoT.Infrastructure.MqttClient.MosquittoWrapper;

namespace SMEIoT.Infrastructure.Services
{
  /// <summary>
  /// must be used as a singleton otherwise it's not thread safe.
  /// The parameters are needed to be configured properly because the error doesn't surface up to CLR.
  /// </summary>
  public class MosquittoClientService : IMosquittoClientService
  {
    public const string BrokerTopic = "$SYS/broker/#";
    public const string SensorTopic = "iot/#";

    public string Host { get; internal set; }
    public int Port { get; internal set; }
    public int KeepAlive { get; internal set; }
    public string? Ciphers { get; internal set; }
    public List<string> Topics { get; internal set; } = new List<string> { BrokerTopic, SensorTopic };
    public int Timeout { get; internal set; }
    public int MaxPackets { get; internal set; }
    public int SleepOnReconnect { get; internal set; }

    public ConnectCallbackDelegate? ConnectCallback { get; internal set; }
    public MessageCallbackDelegate? MessageCallback { get; internal set; }
    
    private readonly IMosquittoClientAuthenticationService _authService;

    public MosquittoClientService(IMosquittoClientAuthenticationService authService, IMqttClientConfigService config, IMosquittoMessageHandler handler)
    {
      _authService = authService;
      Host = config.GetHost();
      Port = config.GetPort();
      KeepAlive = 60;
      Timeout = -1;
      MaxPackets = 1; // document says it's unused and should be set to 1 for future compatibility
      SleepOnReconnect = 10;
      MessageCallback = handler.HandleMessage;
    }

    public async Task Connect()
    {
      // the service is registered as transient, but double invoke this can cause trouble.
      MosquittoWrapper.mosq_init();   

      var psk = await _authService.GetClientPskAsync();
      var identity = await _authService.GetClientNameAsync();
      var res = MosquittoWrapper.mosq_set_tls_psk(psk, identity, Ciphers);
      if (res != 0)
      {
        throw new ArgumentException($"Mosquitto mosq_set_tls_psk returned {res}");
      }
      MosquittoWrapper.mosq_set_callback(ConnectCallback, MessageCallback);

      res = MosquittoWrapper.mosq_connect(Host, Port, KeepAlive);
      if (res != 0)
      {
        throw new TimeoutException($"Mosquitto mosq_connect returned {res}");
      }

      var res_list = Topics.Select(t => MosquittoWrapper.mosq_subscribe_topic(t));
      if (res_list.Any(r => r != 0))
      {
        throw new ArgumentException("Mosquitto mosq_subscribe_topic returned error", string.Join(',', res_list));
      }
    }

    public int RunLoop()
    {
      return MosquittoWrapper.mosq_runloop(Timeout, MaxPackets, SleepOnReconnect);
    }
    
    public int Reconnect()
    {
      return MosquittoWrapper.mosq_reconnect();
    }

    public void Dispose()
    {
      MosquittoWrapper.mosq_destroy();
    }

    public void SubscribeTopic(string topic)
    {
      Topics.Add(topic);
    }

  }
}
