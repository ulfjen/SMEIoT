using System.Threading.Tasks;
using SMEIoT.Core.Entities;
using NodaTime;

namespace SMEIoT.Core.Interfaces
{
  public interface ISensorValueService
  {
    Task AddSensorValueAsync(Sensor sensor, double value, Instant instant);
  }
}
