using System.Threading.Tasks;

namespace SMEIoT.Core.Interfaces
{
  public interface IDeviceSensorIdentifierSuggestService
  {
    Task<string> GenerateRandomIdentifierForDeviceAsync(int numWords);
    string? GetARandomIdentifierCandidatesForSensor(string deviceName);
  }
}
