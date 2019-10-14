using System.Collections.Generic;
using System.Threading.Tasks;
using SMEIoT.Core.Entities;

namespace SMEIoT.Core.Interfaces
{
  public interface ISensorAssignmentService
  {
    IAsyncEnumerable<UserSensor> ListAssignedUserSensorsBySensorName(string sensorName);
    Task<bool> AssignSensorToUserAsync(string sensorName, string username);
    Task<UserSensor> GetUserSensor(string username, string sensorName);
    Task<bool> RevokeSensorFromUserAsync(string sensorName, string username);

  }
}
