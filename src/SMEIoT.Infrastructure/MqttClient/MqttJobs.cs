using System;
using static SMEIoT.Infrastructure.MqttClient.MosquittoWrapper;

namespace SMEIoT.Infrastructure.MqttClient
{
  public static class MqttJobs
  {
    public static void OnMessage(mosquitto_message message)
    {
      Console.WriteLine(message.mid);
    }
  }
}
