using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Exceptions;
using SMEIoT.Core.Helpers;
using SMEIoT.Core.Interfaces;

namespace SMEIoT.Core.Services
{
  public class SensorAssignmentService: ISensorAssignmentService
  {
    private readonly UserManager<User> _userManager;
    private readonly IApplicationDbContext _dbContext;

    public SensorAssignmentService(
      IApplicationDbContext dbContext,
      UserManager<User> userManager)
    {
      _dbContext = dbContext;
      _userManager = userManager;
    }

    public async Task AssignSensorToUserAsync(Sensor sensor, User user)
    {
      if (_dbContext.UserSensors.FirstOrDefault(us => us.SensorId == sensor.Id && us.UserId == user.Id) != null)
      {
        throw new InvalidOperationException($"User {user.UserName} is already assigned to the sensor {sensor.Name}!");
      }
      _dbContext.UserSensors.Add(new UserSensor {UserId = user.Id, SensorId = sensor.Id});
      await _dbContext.SaveChangesAsync();
    }

    public async Task RevokeSensorFromUserAsync(Sensor sensor, User user)
    {
      var us = await _dbContext.UserSensors.SingleOrDefaultAsync(us => us.SensorId == sensor.Id && us.UserId == user.Id);
      if (us == null)
      {
        throw new InvalidOperationException("Assignment cannot be found.");
      }

      _dbContext.UserSensors.Remove(us);
      await _dbContext.SaveChangesAsync();
    }

    public async IAsyncEnumerable<(User, IList<string>)> ListAllowedUsersBySensorAsync(Sensor sensor, int offset, int limit)
    {
      RangeQueryValidations.ValidateRangeQueryParameters(offset, limit);

      var admins = await _userManager.GetUsersInRoleAsync("Admin");
      var adminIds = admins.Select(a => a.Id);
      var yields = 0;
      await foreach (var user in 
        AllowedUsersBySensor(sensor, adminIds)
        .Skip(offset)
        .Take(limit).AsAsyncEnumerable())
      {
        admins.Remove(user);
        var roles = await _userManager.GetRolesAsync(user);
        yields++;
        yield return (user, roles);
      }
      foreach (var admin in admins)
      {
        if (yields++ < limit) {
          var roles = await _userManager.GetRolesAsync(admin);
          yield return (admin, roles);
        } else {
          break;
        }
      }
    }
  
    private IQueryable<User> AllowedUsersBySensor(Sensor sensor, IEnumerable<long> adminIds)
    {
      return _dbContext.Users.Join(
          _dbContext.UserSensors.Where(s => s.SensorId == sensor.Id),
          u => u.Id,
          us => us.UserId,
          (u, us) => u
        )
        .Where(u => !adminIds.Contains(u.Id))
        .Distinct()
        .OrderBy(u => u.Id)
        .AsQueryable();
    }

    public async IAsyncEnumerable<Sensor> ListSensorsByUserAsync(User user, int offset, int limit)
    {
      RangeQueryValidations.ValidateRangeQueryParameters(offset, limit);

      if (await _userManager.IsInRoleAsync(user, "Admin"))
      {
        await foreach (var s in _dbContext.Sensors.Include(s => s.Device).Skip(offset).Take(limit).AsAsyncEnumerable())
        {
          yield return s;
        }
      }
      else
      {
        await foreach (var s in
          SensorsByUserQuery(user)
          .Skip(offset)
          .Take(limit).AsAsyncEnumerable())
        {
          yield return s;
        }
      }
    }

    private IQueryable<Sensor> SensorsByUserQuery(User user)
    {
      return _dbContext.Sensors.Join(
          _dbContext.UserSensors.Where(us => us.UserId == user.Id),
          s => s.Id,
          us => us.SensorId,
          (s, us) => s
        )
        .Distinct()
        .OrderBy(s => s.Id)
        .AsQueryable();
    }

    public async Task<int> NumberOfAllowedUsersBySensorAsync(Sensor sensor)
    {
      var admins = await _userManager.GetUsersInRoleAsync("Admin");
      var adminIds = admins.Select(a => a.Id);
      var normalCnt = await AllowedUsersBySensor(sensor, adminIds).CountAsync();
      return adminIds.Count() + normalCnt;
    }

    public async Task<int> NumberOfSensorsByUserAsync(User user)
    {
      if (await _userManager.IsInRoleAsync(user, "Admin"))
      {
        return await _dbContext.Sensors.CountAsync();
      }
      else
      {
        return await SensorsByUserQuery(user).CountAsync();
      }
    }

    public async Task<bool> CanUserSeeSensorAsync(Sensor sensor, User user)
    {
      if (await _userManager.IsInRoleAsync(user, "Admin")) { return true; }

      var count = _dbContext.UserSensors.Where(us => us.SensorId == sensor.Id && us.UserId == user.Id).Count();
      return count > 0;
    }
  }
}
