using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SMEIoT.Infrastructure.MqttClient
{
  public static class MqttJobs
  {
    public static void OnMessage(int mid, string topic, IntPtr payload, int payloadlen, int qos, int retain)
    {
      byte[] decoded = new byte[payloadlen];
      Marshal.Copy(payload, decoded, 0, payloadlen);
      var d = Encoding.UTF8.GetString(decoded, 0, decoded.Length);
      Console.WriteLine(d);
    }
  }
}
