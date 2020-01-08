namespace SMEIoT.Core.Interfaces
{
  public interface ISecureKeySuggestService
  {
    string GenerateSecureKey(int length);
  }
}
