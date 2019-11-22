using System.Threading.Tasks;

namespace SMEIoT.Core.Interfaces
{
  public interface IDeviceSensorIdentifierSuggester
  {
    Task<string> GenerateRandomIdentifierForDeviceAsync(int numWords);
    Task<string> GenerateRandomIdentifierForSensorAsync(int numWords);

  }
}
