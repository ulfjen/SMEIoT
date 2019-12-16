using System.Text;
using System.Threading.Tasks;

namespace SMEIoT.Core.Interfaces
{
  public interface IMosquittoBrokerMessageService
  {
    Task<StringBuilder> ProcessDecodedMessageAsync(string decoded);
  }
}
