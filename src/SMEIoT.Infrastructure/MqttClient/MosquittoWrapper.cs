using System.Runtime.InteropServices;

namespace SMEIoT.Infrastructure.MqttClient
{
  /// <summary>
  /// The container class for methods we used from mosquitto C library.
  /// P/Invoke will use 10-20 X86 instruction regardless of data marshaling.
  /// We can reduce the marshaling cost.
  /// </summary>
  public static class MosquittoWrapper
  {
    public const string MosquittoDll = "mosq";

    [StructLayout(LayoutKind.Sequential)]
    public struct mosquitto_message
    {
      public int mid;
      public string topic;
      public byte[] payload;
      public int payloadlen;
      public int qos;
      [MarshalAs(UnmanagedType.U1)]
      public bool retain;
    }

    public delegate void ConnectCallbackDelegate(int result);
    public delegate void MessageCallbackDelegate(mosquitto_message mesage);

    [DllImport(MosquittoDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern int mosq_init();

    [DllImport(MosquittoDll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int mosq_set_tls_psk(string psk, string identity, string? ciphers);

    [DllImport(MosquittoDll, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void mosq_set_callback(ConnectCallbackDelegate? connect_callback, MessageCallbackDelegate? message_callback);

    [DllImport(MosquittoDll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int mosq_connect(string host, int port, int keepalive);

    [DllImport(MosquittoDll, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int mosq_subscribe_topic(string topic);

    [DllImport(MosquittoDll, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void mosq_runloop(int timeout, int max_packets, int sleep_on_reconnect);

    [DllImport(MosquittoDll, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int mosq_destroy();
  }
}
