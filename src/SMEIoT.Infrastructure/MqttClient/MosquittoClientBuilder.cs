using System.Collections.Generic;
using static SMEIoT.Infrastructure.MqttClient.MosquittoWrapper;

namespace SMEIoT.Infrastructure.MqttClient
{
  public class MosquittoClientBuilder
  {
    private readonly MosquittoClient _client;
    private List<string> _topics = new List<string>();

    public MosquittoClientBuilder()
    {
      _client = new MosquittoClient();
    }

    public MosquittoClientBuilder SetPskTls(string psk, string identity, string? ciphers = null)
    {
      _client.Psk = psk;
      _client.Identity = identity;
      _client.Ciphers = ciphers;
      return this;
    }

    public MosquittoClientBuilder SetConnectCallback(ConnectCallbackDelegate callback)
    {
      _client.ConnectCallback = callback;
      return this;
    }

    public MosquittoClientBuilder SetMessageCallback(MessageCallbackDelegate callback)
    {
      _client.MessageCallback = callback;
      return this;
    }

    public MosquittoClientBuilder SetConnectionInfo(string host, int port)
    {
      _client.Host = host;
      _client.Port = port;
      return this;
    }

    public MosquittoClientBuilder SetKeepAlive(int keepAlive)
    {
      _client.KeepAlive = keepAlive;
      return this;
    }

    public MosquittoClientBuilder SubscribeTopic(string topic)
    {
      _topics.Add(topic);
      return this;
    }

    public MosquittoClientBuilder SetRunLoopInfo(int timeout, int maxPackets, int sleepOnReconnect)
    {
      _client.Timeout = timeout;
      _client.MaxPackets = maxPackets;
      _client.SleepOnReconnect = sleepOnReconnect;
      return this;
    }

    public MosquittoClient Client {
      get {
        _client.Topics = _topics;
        return _client;
      }
    }
  }
}
