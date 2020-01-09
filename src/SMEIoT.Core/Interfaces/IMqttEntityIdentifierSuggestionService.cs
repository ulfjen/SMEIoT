using System.Threading.Tasks;

namespace SMEIoT.Core.Interfaces
{
  public interface IMqttEntityIdentifierSuggestionService
  {
    Task<string> GenerateRandomIdentifierForDeviceAsync(int numWords);
    string? GetARandomIdentifierCandidatesForSensor(string deviceName);
  }
}
