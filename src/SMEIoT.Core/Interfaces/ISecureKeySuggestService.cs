using System.Threading.Tasks;

namespace SMEIoT.Core.Interfaces
{
  public interface ISecureKeySuggestService
  {
    Task<string> GenerateSecureKeyWithByteLengthAsync(int length);
  }
}
