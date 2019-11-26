namespace SMEIoT.Core.Interfaces
{
  public interface IDeviceSensorIdentifierSuggestService
  {
    string GenerateRandomIdentifierForDevice(int numWords);
    string? GetARandomIdentifierCandidatesForSensor(string deviceName);
  }
}
