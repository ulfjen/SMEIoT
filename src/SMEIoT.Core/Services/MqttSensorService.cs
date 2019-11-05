using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Exceptions;
using SMEIoT.Core.Interfaces;

namespace SMEIoT.Core.Services
{
  public class MqttSensorService : IMqttSensorService
  {
    public MqttSensorService()
    {
    }

    public IAsyncEnumerable<string> ListSensorNames(string pattern)
    {
      throw new System.NotImplementedException();
    }

    public Task<bool> RegisterSensorByName(string sensorName, Period expiredIn)
    {
      throw new System.NotImplementedException();
    }
  }
}
