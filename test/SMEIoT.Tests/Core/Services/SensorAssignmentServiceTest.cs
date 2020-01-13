using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Exceptions;
using SMEIoT.Core.Services;
using SMEIoT.Infrastructure.Data;
using SMEIoT.Tests.Shared;
using Xunit;

namespace SMEIoT.Tests.Core.Services
{
  public class SensorAssignmentServiceTest
  {
    private static async Task<(SensorAssignmentService, ApplicationDbContext, UserManager<User>)> BuildService(bool fabricateData = true)
    {
      var dbContext = ApplicationDbContextHelper.BuildTestDbContext();
      var (userManager, roleManager) = MockHelpers.CreateUserManager();

      if (fabricateData)
      {
        foreach (var sensorName in new[] {"sensor-1", "sensor-2", "sensor-3"})
        {
          dbContext.Sensors.Add(new Sensor {Name = sensorName, NormalizedName = sensorName.ToUpperInvariant()});
        }

        await dbContext.SaveChangesAsync();
        await Task.WhenAll(new[] {"normal-user-1", "normal-user-2", "normal-user-3"}.Select(userName =>
          userManager.CreateAsync(
            new User
            {
              UserName = userName,
              NormalizedUserName = userName.ToUpperInvariant(),
              SecurityStamp = Guid.NewGuid().ToString()
            },
            "normal-password")));
      }

      return (new SensorAssignmentService(dbContext, userManager), dbContext, userManager);
    }

    [Fact]
    public async Task AssignSensorToUserAsync_ThrowsErrorsIfNoSensor()
    {
      // arrange
      var (service, dbContext, userManager) = await BuildService();

      // act
      Task Act() => service.AssignSensorToUserAsync("not-exist-sensor", "normal-user-1");

      // assert
      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var notFound = Assert.IsType<EntityNotFoundException>(exce);
      Assert.Equal("sensorName", notFound.ParamName);
    }
    
    [Fact]
    public async Task AssignSensorToUserAsync_ThrowsErrorsIfNoUser()
    {
      var (service, dbContext, userManager) = await BuildService();

      Task Act() => service.AssignSensorToUserAsync("sensor-1", "not-existing-user-1");

      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var notFound = Assert.IsType<EntityNotFoundException>(exce);
      Assert.Equal("userName", notFound.ParamName);
    }

    [Fact]
    public async Task AssignSensorToUserAsync_Assigns()
    {
      var (service, dbContext, userManager) = await BuildService();

      var act = await service.AssignSensorToUserAsync("sensor-1", "normal-user-1");

      Assert.True(act);
    }
    
    [Fact]
    public async Task ListAssignedUserSensorsBySensorName_ThrowsErrorsIfNoSensor()
    {
      var (service, dbContext, userManager) = await BuildService();

      Func<Task> act = async () =>
      {
        await foreach (var _ in service.ListAssignedUserSensorsBySensorName("not-exist-sensor")) { }
      };

      var exce = await Record.ExceptionAsync(act);
      Assert.NotNull(exce);
      var notFound = Assert.IsType<EntityNotFoundException>(exce);
      Assert.Equal("sensorName", notFound.ParamName);
    }

    [Fact]
    public async Task ListAssignedUserSensorsBySensorName_ReturnsUserSensors()
    {
      var (service, dbContext, userManager) = await BuildService();
      var sensor = await dbContext.Sensors.Where(s => s.NormalizedName == Sensor.NormalizeName("sensor-1"))
        .FirstAsync();
      var user1 = await userManager.FindByNameAsync("normal-user-1");
      var user2 = await userManager.FindByNameAsync("normal-user-2");
      dbContext.UserSensors.Add(new UserSensor
      {
        UserId = user1.Id, SensorId = sensor.Id
      });
      dbContext.UserSensors.Add(new UserSensor
      {
        UserId = user2.Id,
        SensorId = sensor.Id
      });
      await dbContext.SaveChangesAsync();

      var userSensors = new List<UserSensor>();
      await foreach (var us in service.ListAssignedUserSensorsBySensorName("sensor-1"))
      {
        userSensors.Add(us);
      }
      
      Assert.Equal(2, userSensors.Count);
      Assert.Contains(userSensors, x => x.UserId == user1.Id);
      Assert.Contains(userSensors, x => x.UserId == user2.Id);
    }
    
    [Fact]
    public async Task GetUserSensor_ThrowsErrorsIfNoSensor()
    {
      // arrange
      var (service, dbContext, userManager) = await BuildService();

      // act
      Task Act() => service.GetUserSensor("not-exist-sensor", "normal-user-1");

      // assert
      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var notFound = Assert.IsType<EntityNotFoundException>(exce);
      Assert.Equal("sensorName", notFound.ParamName);
    }
    
    
    [Fact]
    public async Task GetUserSensor_ThrowsErrorsIfNoUserSensor()
    {
      var (service, dbContext, userManager) = await BuildService();

      Task Act() => service.GetUserSensor("normal-user-1", "sensor-1");

      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var notFound = Assert.IsType<EntityNotFoundException>(exce);
      Assert.Equal("userSensor", notFound.ParamName);
    }

    
    [Fact]
    public async Task GetUserSensor_ThrowsErrorsIfNoUser()
    {
      var (service, dbContext, userManager) = await BuildService();

      Task Act() => service.GetUserSensor("not-existing-user-1", "sensor-1");

      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var notFound = Assert.IsType<EntityNotFoundException>(exce);
      Assert.Equal("userName", notFound.ParamName);
    }

    
    [Fact]
    public async Task GetUserSensor_ReturnsUserSensor()
    {
      var (service, dbContext, userManager) = await BuildService();
      var sensor = await dbContext.Sensors.Where(s => s.NormalizedName == Sensor.NormalizeName("sensor-1"))
        .FirstAsync();
      var user = await userManager.FindByNameAsync("normal-user-1");
      dbContext.UserSensors.Add(new UserSensor
      {
        UserId = user.Id, SensorId = sensor.Id
      });
      await dbContext.SaveChangesAsync();

      var us = await service.GetUserSensor(user.UserName, sensor.Name);
      
      Assert.Equal(sensor.Id, us.SensorId);
      Assert.Equal(user.Id, us.UserId);
    }
    
    
    [Fact]
    public async Task RevokeSensorFromUserAsync_ThrowsErrorsIfNoSensor()
    {
      // arrange
      var (service, dbContext, userManager) = await BuildService();

      // act
      Task Act() => service.RevokeSensorFromUserAsync("not-exist-sensor", "normal-user-1");

      // assert
      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var notFound = Assert.IsType<EntityNotFoundException>(exce);
      Assert.Equal("sensorName", notFound.ParamName);
    }
    
    [Fact]
    public async Task RevokeSensorFromUserAsync_ThrowsErrorsIfNoUser()
    {
      var (service, dbContext, userManager) = await BuildService();

      Task Act() => service.RevokeSensorFromUserAsync("sensor-1", "not-existing-user-1");

      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var notFound = Assert.IsType<EntityNotFoundException>(exce);
      Assert.Equal("userName", notFound.ParamName);
    }
    
    [Fact]
    public async Task RevokeSensorFromUserAsync_ThrowsErrorsIfNoUserSensor()
    {
      var (service, dbContext, userManager) = await BuildService();

      Task Act() => service.RevokeSensorFromUserAsync("sensor-1", "normal-user-1");

      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var notFound = Assert.IsType<EntityNotFoundException>(exce);
      Assert.Equal("userSensor", notFound.ParamName);
    }
    
    [Fact]
    public async Task RevokeSensorFromUserAsync_Revokes()
    {
      var (service, dbContext, userManager) = await BuildService();
      var sensor = await dbContext.Sensors.Where(s => s.NormalizedName == Sensor.NormalizeName("sensor-1"))
        .FirstAsync();
      var user = await userManager.FindByNameAsync("normal-user-1");
      dbContext.UserSensors.Add(new UserSensor
      {
        UserId = user.Id, SensorId = sensor.Id
      });
      await dbContext.SaveChangesAsync();

      var us = await service.RevokeSensorFromUserAsync(sensor.Name, user.UserName);
      
      Assert.True(us);
    }
  }
}
