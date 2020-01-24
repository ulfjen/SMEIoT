using System.Collections.Generic;
using System.Threading.Tasks;
using SMEIoT.Core.Entities;

namespace SMEIoT.Core.Interfaces
{
  public interface ISensorAssignmentService
  {
    IAsyncEnumerable<(User, IList<string>)> ListAllowedUsersBySensorAsync(Sensor sensor);
    IAsyncEnumerable<Sensor> ListSensorsByUserNameAsync(User user);
    Task AssignSensorToUserAsync(Sensor sensor, User user);
    Task RevokeSensorFromUserAsync(Sensor sensor, User user);
    Task<bool> CanUserSeeSensorAsync(Sensor sensor, User user);
  }
}
