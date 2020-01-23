using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using SMEIoT.Core.Interfaces;

namespace SMEIoT.Infrastructure.Services
{
  public class ServerNetworkInterfacesIpAccessor : IServerNetworkInterfacesIpAccessor
  {
    public IList<string> GetNetworkInterfacesIpString()
    {
      var ipList = new List<string>();
      foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces())
      {
        foreach (var address in netInterface.GetIPProperties().UnicastAddresses)
        {
          var ipAddress = address.Address;
          if ((ipAddress.AddressFamily == AddressFamily.InterNetwork || ipAddress.AddressFamily == AddressFamily.InterNetworkV6) && !IPAddress.IsLoopback(ipAddress))
          {
            ipList.Add(ipAddress.ToString());
          }
        }
      }
      return ipList;
    }
  }
}
