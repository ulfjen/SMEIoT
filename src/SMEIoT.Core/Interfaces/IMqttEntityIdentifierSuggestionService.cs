using System.Threading.Tasks;

namespace SMEIoT.Core.Interfaces
{
  public interface IMqttEntityIdentifierSuggestionService
  {
    Task<string> GenerateRandomIdentifierForDeviceAsync(int numWords);
    Task<string?> GetOneIdentifierCandidateForSensorAsync(string deviceName);
  }
}
