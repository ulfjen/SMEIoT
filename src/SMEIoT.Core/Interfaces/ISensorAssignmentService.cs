using System.Collections.Generic;
using System.Threading.Tasks;
using SMEIoT.Core.Entities;

namespace SMEIoT.Core.Interfaces
{
  public interface ISensorAssignmentService
  {
    IAsyncEnumerable<User> ListAllowedUsersBySensorNameAsync(string sensorName);
    IAsyncEnumerable<Sensor> ListSensorsByUserNameAsync(string userName);
    Task AssignSensorToUserAsync(string sensorName, string userName);
    Task RevokeSensorFromUserAsync(string sensorName, string userName);
    Task<bool> DoesUserAllowToSeeSensorAsync(string sensorName, string userName);
  }
}
