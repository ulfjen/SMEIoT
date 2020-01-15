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
using SMEIoT.Web.Services;
using Xunit;

namespace SMEIoT.Tests.Core.Services
{
  [Collection("Database collection")]
#pragma warning disable CA1063 // Implement IDisposable Correctly
  public class SensorAssignmentServiceTest : IDisposable
#pragma warning restore CA1063 // Implement IDisposable Correctly
  {
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager _userManager;
    private readonly SensorAssignmentService _service;

    public SensorAssignmentServiceTest()
    {
      _dbContext = ApplicationDbContextHelper.BuildTestDbContext();
      _userManager = IdentityHelpers.BuildUserManager(ApplicationDbContextHelper.BuildTestDbContext());
      _service = new SensorAssignmentService(_dbContext, _userManager);
    }

#pragma warning disable CA1063 // Implement IDisposable Correctly
    public void Dispose()
#pragma warning restore CA1063 // Implement IDisposable Correctly
    {
      _userManager.Dispose();
      _dbContext.Database.ExecuteSqlInterpolated($"TRUNCATE users, user_sensors, sensors, devices RESTART IDENTITY CASCADE;");
      _dbContext.Dispose();
    }

    private async Task SeedDefaultCaseAsync()
    {
      var roleManager = IdentityHelpers.BuildRoleManager(ApplicationDbContextHelper.BuildTestDbContext());
      await roleManager.CreateAsync(new IdentityRole<long>("Admin"));

      foreach (var userName in new[] { "normal-user-1", "normal-user-2", "normal-user-3", "admin-user" })
      {
        await _userManager.CreateAsync(new User
         {
           UserName = userName,
           NormalizedUserName = userName.ToUpperInvariant(),
           SecurityStamp = Guid.NewGuid().ToString()
         }, "normal-password");
      }
      var admin = await _userManager.FindByNameAsync("admin-user");
      await _userManager.AddToRoleAsync(admin, "Admin");

      var deviceName = "device";
      var device = new Device {
        Name = deviceName,
        NormalizedName = deviceName.ToUpperInvariant(),
        AuthenticationType = DeviceAuthenticationType.PreSharedKey,
        PreSharedKey = "key"
      };
      _dbContext.Devices.Add(device);

      var name1 = "sensor-1";
      var sensor1 = new Sensor { Name = name1, NormalizedName = name1.ToUpperInvariant(), Device = device };
      _dbContext.Sensors.Add(sensor1);
      var name2 = "sensor-2";
      var sensor2 = new Sensor { Name = name2, NormalizedName = name2.ToUpperInvariant(), Device = device };
      _dbContext.Sensors.Add(sensor2);

      var user1 = await _userManager.FindByNameAsync("normal-user-1");
      var user2 = await _userManager.FindByNameAsync("normal-user-2");
      _dbContext.UserSensors.Add(new UserSensor { UserId = user1.Id, Sensor = sensor1 });
      _dbContext.UserSensors.Add(new UserSensor { UserId = user1.Id, Sensor = sensor2 });
      _dbContext.UserSensors.Add(new UserSensor { UserId = user2.Id, Sensor = sensor1 });

      await _dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task AssignSensorToUserAsync_ThrowsErrorsIfNoSensor()
    {
      // arrange
      await SeedDefaultCaseAsync();

      // act
      Task Act() => _service.AssignSensorToUserAsync("not-exist-sensor", "normal-user-1");

      // assert
      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var notFound = Assert.IsType<EntityNotFoundException>(exce);
      Assert.Equal("sensorName", notFound.ParamName);
    }
    
    [Fact]
    public async Task AssignSensorToUserAsync_ThrowsErrorsIfNoUser()
    {
      await SeedDefaultCaseAsync();

      Task Act() => _service.AssignSensorToUserAsync("sensor-1", "not-existing-user-1");

      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var notFound = Assert.IsType<EntityNotFoundException>(exce);
      Assert.Equal("userName", notFound.ParamName);
    }

    [Fact]
    public async Task AssignSensorToUserAsync_Assigns()
    {
      await SeedDefaultCaseAsync();

      await _service.AssignSensorToUserAsync("sensor-1", "normal-user-3");

      // throws nothing
    }

    [Fact]
    public async Task AssignSensorToUserAsync_ThrowsIfAlreadyAssigned()
    {
      await SeedDefaultCaseAsync();

      Task Act() => _service.AssignSensorToUserAsync("sensor-1", "normal-user-1");

      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var details = Assert.IsType<InvalidOperationException>(exce);
    }

    [Fact]
    public async Task RevokeSensorFromUserAsync_ThrowsErrorsIfNoSensor()
    {
      await SeedDefaultCaseAsync();

      Task Act() => _service.RevokeSensorFromUserAsync("not-exist-sensor", "normal-user-1");

      // assert
      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var notFound = Assert.IsType<EntityNotFoundException>(exce);
      Assert.Equal("sensorName", notFound.ParamName);
    }

    [Fact]
    public async Task RevokeSensorFromUserAsync_ThrowsErrorsIfNoUser()
    {
      await SeedDefaultCaseAsync();

      Task Act() => _service.RevokeSensorFromUserAsync("sensor-1", "not-existing-user-1");

      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var notFound = Assert.IsType<EntityNotFoundException>(exce);
      Assert.Equal("userName", notFound.ParamName);
    }

    [Fact]
    public async Task RevokeSensorFromUserAsync_ThrowsErrorsIfNoAssignment()
    {
      await SeedDefaultCaseAsync();

      Task Act() => _service.RevokeSensorFromUserAsync("sensor-1", "normal-user-3");

      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var details = Assert.IsType<InvalidOperationException>(exce);
      Assert.Contains("Assignment", details.Message);
    }

    [Fact]
    public async Task RevokeSensorFromUserAsync_Revokes()
    {
      await SeedDefaultCaseAsync();

      await _service.RevokeSensorFromUserAsync("sensor-1", "normal-user-1");
    }

    [Fact]
    public async Task DoesUserAllowToSeeSensorAsync_ThrowsErrorsIfNoSensor()
    {
      await SeedDefaultCaseAsync();

      Task Act() => _service.DoesUserAllowToSeeSensorAsync("not-exist-sensor", "normal-user-1");

      // assert
      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var notFound = Assert.IsType<EntityNotFoundException>(exce);
      Assert.Equal("sensorName", notFound.ParamName);
    }

    [Fact]
    public async Task DoesUserAllowToSeeSensorAsync_ThrowsErrorsIfNoUser()
    {
      await SeedDefaultCaseAsync();

      Task Act() => _service.DoesUserAllowToSeeSensorAsync("sensor-1", "not-existing-user-1");

      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var notFound = Assert.IsType<EntityNotFoundException>(exce);
      Assert.Equal("userName", notFound.ParamName);
    }

    [Fact]
    public async Task DoesUserAllowToSeeSensorAsync_ReturnsFalseIfNoAssignment()
    {
      await SeedDefaultCaseAsync();

      var result = await _service.DoesUserAllowToSeeSensorAsync("sensor-1", "normal-user-3");

      Assert.False(result);
    }

    [Fact]
    public async Task DoesUserAllowToSeeSensorAsync_ReturnsTrue()
    {
      await SeedDefaultCaseAsync();

      var result = await _service.DoesUserAllowToSeeSensorAsync("sensor-1", "normal-user-1");

      Assert.True(result);
    }

    [Fact]
    public async Task DoesUserAllowToSeeSensorAsync_ReturnsTrueForAdmin()
    {
      await SeedDefaultCaseAsync();

      Assert.True(await _service.DoesUserAllowToSeeSensorAsync("sensor-1", "admin-user"));
      Assert.True(await _service.DoesUserAllowToSeeSensorAsync("sensor-2", "admin-user"));
    }

    [Fact]
    public async Task ListAllowedUsersBySensorNameAsync_ThrowsErrorsIfNoSensor()
    {
      await SeedDefaultCaseAsync();

      Func<Task> act = async () =>
      {
        await foreach (var _ in _service.ListAllowedUsersBySensorNameAsync("not-exist-sensor")) { }
      };

      var exce = await Record.ExceptionAsync(act);
      Assert.NotNull(exce);
      var notFound = Assert.IsType<EntityNotFoundException>(exce);
      Assert.Equal("sensorName", notFound.ParamName);
    }

    [Fact]
    public async Task ListAllowedUsersBySensorNameAsync_ReturnsUsers()
    {
      await SeedDefaultCaseAsync();

      var users = new List<User>();
      await foreach (var u in _service.ListAllowedUsersBySensorNameAsync("sensor-1"))
      {
        users.Add(u);
      }
      
      Assert.Equal(3, users.Count);
      Assert.Contains(users, x => x.UserName == "normal-user-1");
      Assert.Contains(users, x => x.UserName == "normal-user-2");
      Assert.Contains(users, x => x.UserName == "admin-user");
    }

    [Fact]
    public async Task ListAllowedUsersBySensorNameAsync_ReturnsEmptyList()
    {
      await SeedDefaultCaseAsync();

      var users = new List<User>();
      await foreach (var u in _service.ListAllowedUsersBySensorNameAsync("sensor-1"))
      {
      }

      Assert.Empty(users);
    }

    [Fact]
    public async Task ListSensorsByUserNameAsync_ThrowsErrorsIfNoUser()
    {
      await SeedDefaultCaseAsync();

      Func<Task> act = async () =>
      {
        await foreach (var _ in _service.ListSensorsByUserNameAsync("not-exist-user")) { }
      };

      var exce = await Record.ExceptionAsync(act);
      Assert.NotNull(exce);
      var notFound = Assert.IsType<EntityNotFoundException>(exce);
      Assert.Equal("userName", notFound.ParamName);
    }

    [Fact]
    public async Task ListSensorsByUserNameAsync_ReturnsSensors()
    {
      await SeedDefaultCaseAsync();

      var sensors = new List<Sensor>();
      await foreach (var u in _service.ListSensorsByUserNameAsync("normal-user-1"))
      {
        sensors.Add(u);
      }

      Assert.Equal(2, sensors.Count);
      Assert.Contains(sensors, x => x.Name == "sensor-1");
      Assert.Contains(sensors, x => x.Name == "sensor-2");
    }

    [Fact]
    public async Task ListSensorsByUserNameAsync_ReturnsEmptyList()
    {
      await SeedDefaultCaseAsync();

      var sensors = new List<Sensor>();
      await foreach (var u in _service.ListSensorsByUserNameAsync("normal-user-3"))
      {
        sensors.Add(u);
      }

      Assert.Empty(sensors);
    }

    [Fact]
    public async Task ListSensorsByUserNameAsync_ReturnsEverythingForAdmin()
    {
      await SeedDefaultCaseAsync();

      var sensors = new List<Sensor>();
      await foreach (var u in _service.ListSensorsByUserNameAsync("admin-user"))
      {
        sensors.Add(u);
      }

      Assert.Equal(_dbContext.Sensors.Count(), sensors.Count());
    }
  }
}
