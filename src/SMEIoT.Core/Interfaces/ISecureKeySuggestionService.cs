using System.Threading.Tasks;

namespace SMEIoT.Core.Interfaces
{
  public interface ISecureKeySuggestionService
  {
    Task<string> GenerateSecureKeyWithByteLengthAsync(int length);
  }
}
