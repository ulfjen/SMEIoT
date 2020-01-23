using System.Threading.Tasks;
using System.Collections.Generic;

namespace SMEIoT.Core.Interfaces
{
  public interface IServerHostAccessor
  {
    // usually mqtt client is running with the server. 
    // we can get server host or IP instead of localhost.
    // deployment configuration will affect this API
    Task<string?> GetServerHostAsync();
  }
}
