using System.Threading.Tasks;
using System.Collections.Generic;

namespace SMEIoT.Core.Interfaces
{
  public interface IServerNetworkInterfacesIpAccessor
  {
    IList<string> GetNetworkInterfacesIpString();
  }
}
