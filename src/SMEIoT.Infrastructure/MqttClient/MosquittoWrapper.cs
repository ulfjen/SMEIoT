using System;
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

    public delegate void ConnectCallbackDelegate(int result);
    public delegate void MessageCallbackDelegate(int mid, string topic, IntPtr payload, int payloadlen, int qos, int retain);

    [DllImport(MosquittoDll, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int mosq_init();

    [DllImport(MosquittoDll, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int mosq_set_tls_psk(string psk, string identity, string? ciphers);

    [DllImport(MosquittoDll, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void mosq_set_callback(ConnectCallbackDelegate? connect_callback, MessageCallbackDelegate? message_callback);

    [DllImport(MosquittoDll, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int mosq_connect(string host, int port, int keepalive);

    [DllImport(MosquittoDll, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int mosq_subscribe_topic(string topic);

    [DllImport(MosquittoDll, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void mosq_runloop(int timeout, int max_packets, int sleep_on_reconnect);

    [DllImport(MosquittoDll, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int mosq_destroy();
  }
}
