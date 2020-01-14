using System;
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

    public async Task AssignSensorToUserAsync(string sensorName, string userName)
    {
      var (sensor, user) = await GetUserAndSensor(sensorName, userName);

      _dbContext.UserSensors.Add(new UserSensor {UserId = user.Id, SensorId = sensor.Id});
      await _dbContext.SaveChangesAsync();
    }

    public async Task RevokeSensorFromUserAsync(string sensorName, string userName)
    {
      var (sensor, user) = await GetUserAndSensor(sensorName, userName);

      var us = await _dbContext.UserSensors.SingleOrDefaultAsync(us => us.SensorId == sensor.Id && us.UserId == user.Id);
      if (us == null)
      {
        throw new InvalidOperationException("Assignment cannot be found.");
      }

      _dbContext.UserSensors.Remove(us);
      await _dbContext.SaveChangesAsync();
    }

    private async Task<(Sensor, User)> GetUserAndSensor(string sensorName, string userName)
    {
      var sensor = await GetSensorByNameAsync(sensorName);
      if (sensor == null)
      {
        throw new EntityNotFoundException("Sensor cannot be found.", nameof(sensorName));
      }

      var user = await GetUserByNameAsync(userName);
      if (user == null)
      {
        throw new EntityNotFoundException("User cannot be found.", nameof(userName));
      }

      return (sensor, user);
    }

    private async Task<Sensor?> GetSensorByNameAsync(string name)
    {
      var normalized = Sensor.NormalizeName(name);
      return await _dbContext.Sensors.Where(s => s.NormalizedName == normalized).FirstOrDefaultAsync();
    }

    private async Task<User?> GetUserByNameAsync(string name)
    {
      return await _userManager.FindByNameAsync(name);
    }

    public async IAsyncEnumerable<User> ListAllowedUsersBySensorNameAsync(string sensorName)
    {
      var sensor = await GetSensorByNameAsync(sensorName);
      if (sensor == null)
      {
        throw new EntityNotFoundException("cannot find the sensor.", nameof(sensorName));
      }
      var admins = await _userManager.GetUsersInRoleAsync("Admin");
      await foreach (var user in _dbContext.Users.Join(_dbContext.UserSensors, u => u.Id, us => us.UserId, (u, us) => u).Distinct().AsAsyncEnumerable())
      {
        admins.Remove(user);
        yield return user;
      }
      foreach (var admin in admins)
      {
        yield return admin;
      }
    }

    public async IAsyncEnumerable<Sensor> ListSensorsByUserNameAsync(string userName)
    {
      var user = await GetUserByNameAsync(userName);
      if (user == null)
      {
        throw new EntityNotFoundException("User can't be found.", nameof(userName));
      }

      if (await _userManager.IsInRoleAsync(user, "Admin"))
      {
        await foreach (var s in _dbContext.Sensors)
        {
          yield return s;
        }
      }
      else
      {
        var userSensors = _dbContext.UserSensors.Where(us => us.UserId == user.Id).AsQueryable();
        await foreach (var s in _dbContext.Sensors.Join(userSensors, s => s.Id, us => us.SensorId, (s, us) => s).Distinct().AsAsyncEnumerable())
        {
          yield return s;
        }
      }
    }

    public async Task<bool> DoesUserAllowToSeeSensorAsync(string sensorName, string userName)
    {
      var (sensor, user) = await GetUserAndSensor(sensorName, userName);

      if (await _userManager.IsInRoleAsync(user, "Admin")) { return true; }

      var count = _dbContext.UserSensors.Where(us => us.SensorId == sensor.Id && us.UserId == user.Id).Count();
      return count > 0;
    }
  }
}
