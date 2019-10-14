using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Exceptions;
using SMEIoT.Core.Interfaces;

namespace SMEIoT.Core.Services
{
  public class SensorAssignmentService: ISensorAssignmentService
  {
    private readonly UserManager<User> _userManager;
    private readonly IApplicationDbContext _dbContext;

    public SensorAssignmentService(IApplicationDbContext dbContext,
      UserManager<User> userManager)
    {
      _dbContext = dbContext;
      _userManager = userManager;
    }

    public async IAsyncEnumerable<UserSensor> ListAssignedUserSensorsBySensorName(string sensorName)
    {
      var sensor = await GetSensorByNameAsync(sensorName);
      if (sensor == null)
      {
        throw new EntityNotFoundException("cannot find the sensor.", "sensorName");
      }
      foreach (var userSensor in _dbContext.UserSensors.Where(us => us.Sensor == sensor))
      {
        yield return userSensor;
      }
    }

    public async Task<bool> AssignSensorToUserAsync(string sensorName, string username)
    {
      var (sensor, user) = await GetUserAndSensor(sensorName, username);

      _dbContext.UserSensors.Add(new UserSensor {UserId = user.Id, SensorId = sensor.Id});
      await _dbContext.SaveChangesAsync();
      return true;
    }

    public async Task<UserSensor> GetUserSensor(string username, string sensorName)
    {
      var (sensor, user) = await GetUserAndSensor(sensorName, username);
      var us = await GetUserSensor(user, sensor);
      if (us == null)
      {
        throw new EntityNotFoundException("cannot be found.", "userSensor");
      }

      return us;
    }

    public async Task<bool> RevokeSensorFromUserAsync(string sensorName, string username)
    {
      var (sensor, user) = await GetUserAndSensor(sensorName, username);

      var userSensor = await GetUserSensor(user, sensor);
      if (userSensor == null)
      {
        throw new EntityNotFoundException("cannot be found", "userSensor");
      }

      _dbContext.UserSensors.Remove(userSensor);
      await _dbContext.SaveChangesAsync();
      return true;
    }

    private Task<UserSensor> GetUserSensor(User user, Sensor sensor)
    {
      return _dbContext.UserSensors.SingleOrDefaultAsync(us => us.SensorId == sensor.Id && us.UserId == user.Id);
    }

    private async Task<(Sensor, User)> GetUserAndSensor(string sensorName, string username)
    {
      var sensor = await GetSensorByNameAsync(sensorName);
      if (sensor == null)
      {
        throw new EntityNotFoundException("cannot be found.", "sensorName");
      }

      var user = await GetUserByNameAsync(username);
      if (user == null)
      {
        throw new EntityNotFoundException("cannot be found.", "userName");
      }

      return (sensor, user);
    }

    private Task<Sensor> GetSensorByNameAsync(string name)
    {
      return _dbContext.Sensors.Where("NormalizedName = @0", Sensor.NormalizeName(name)).FirstOrDefaultAsync();
    }

    private Task<User> GetUserByNameAsync(string name)
    {
      return _userManager.FindByNameAsync(name);
    }
  }
}
