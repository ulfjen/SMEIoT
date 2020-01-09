using System.Threading.Tasks;

namespace SMEIoT.Core.Interfaces
{
  public interface IMosquittoClientAuthenticationService
  {
    Task<string> GetClientNameAsync();
    Task<string> GetClientPskAsync();
  }
}
