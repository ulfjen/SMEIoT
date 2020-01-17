using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NodaTime;

namespace SMEIoT.Core.Interfaces
{
  public interface IMosquittoMessageHandler
  {
    void HandleMessage(int mid, string topic, IntPtr payload, int payloadlen, int qos, int retain);
  }
}
