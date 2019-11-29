namespace SMEIoT.Core.Interfaces
{
  public interface ISecureKeySuggestService
  {
    string GenerateSecureKeyWithByteLength(int length);
  }
}
