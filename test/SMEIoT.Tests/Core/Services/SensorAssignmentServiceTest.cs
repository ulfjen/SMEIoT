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

    private async Task<(Sensor, Sensor)> SeedDefaultCaseAsync()
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
      return (sensor1, sensor2);
    }

    [Fact]
    public async Task AssignSensorToUserAsync_Assigns()
    {
      var (sensor1, sensor2) = await SeedDefaultCaseAsync();
      var user = await _userManager.FindByNameAsync("normal-user-3");

      await _service.AssignSensorToUserAsync(sensor1, user);

      // throws nothing
    }

    [Fact]
    public async Task AssignSensorToUserAsync_ThrowsIfAlreadyAssigned()
    {
      var (sensor1, sensor2) = await SeedDefaultCaseAsync();
      var user = await _userManager.FindByNameAsync("normal-user-1");

      Task Act() => _service.AssignSensorToUserAsync(sensor1, user);

      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var details = Assert.IsType<InvalidOperationException>(exce);
      Assert.Contains("already assigned", details.Message);
    }

    [Fact]
    public async Task RevokeSensorFromUserAsync_ThrowsErrorsIfNoAssignment()
    {
      var (sensor1, sensor2) = await SeedDefaultCaseAsync();
      var user = await _userManager.FindByNameAsync("normal-user-3");

      Task Act() => _service.RevokeSensorFromUserAsync(sensor1, user);

      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var details = Assert.IsType<InvalidOperationException>(exce);
      Assert.Contains("Assignment", details.Message);
    }

    [Fact]
    public async Task RevokeSensorFromUserAsync_Revokes()
    {
      var (sensor1, sensor2) = await SeedDefaultCaseAsync();
      var user = await _userManager.FindByNameAsync("normal-user-1");

      await _service.RevokeSensorFromUserAsync(sensor1, user);
    }

    [Fact]
    public async Task CanUserSeeSensorAsync_ReturnsFalseIfNoAssignment()
    {
      var (sensor1, sensor2) = await SeedDefaultCaseAsync();
      var user = await _userManager.FindByNameAsync("normal-user-3");

      var result = await _service.CanUserSeeSensorAsync(sensor1, user);

      Assert.False(result);
    }

    [Fact]
    public async Task CanUserSeeSensorAsync_ReturnsTrue()
    {
      var (sensor1, sensor2) = await SeedDefaultCaseAsync();
      var user = await _userManager.FindByNameAsync("normal-user-1");

      var result = await _service.CanUserSeeSensorAsync(sensor1, user);

      Assert.True(result);
    }

    [Fact]
    public async Task CanUserSeeSensorAsync_ReturnsTrueForAdmin()
    {
      var (sensor1, sensor2) = await SeedDefaultCaseAsync();
      var admin = await _userManager.FindByNameAsync("admin-user");

      Assert.True(await _service.CanUserSeeSensorAsync(sensor1, admin));
      Assert.True(await _service.CanUserSeeSensorAsync(sensor2, admin));
    }

    [Fact]
    public async Task ListAllowedUsersBySensorAsync_ReturnsUsers()
    {
      var (sensor1, sensor2) = await SeedDefaultCaseAsync();

      var users = new List<(User, IList<string>)>();
      await foreach (var (u, r) in _service.ListAllowedUsersBySensorAsync(sensor1, 0, 10))
      {
        users.Add((u, r));
      }
      
      Assert.Equal(3, users.Count);
      Assert.Contains(users, x => x.Item1.UserName == "normal-user-1");
      Assert.Contains(users, x => x.Item1.UserName == "normal-user-2");
      Assert.Contains(users, x => x.Item1.UserName == "admin-user" && x.Item2.Contains("Admin"));
    }

    [Fact]
    public async Task ListAllowedUsersBySensorAsync_ReturnsEmpty()
    {
      var deviceName = "device";
      var device = new Device {
        Name = deviceName,
        NormalizedName = deviceName.ToUpperInvariant(),
        AuthenticationType = DeviceAuthenticationType.PreSharedKey,
        PreSharedKey = "key"
      };
      _dbContext.Devices.Add(device);

      var name1 = "sensor-1";
      var sensor = new Sensor { Name = name1, NormalizedName = name1.ToUpperInvariant(), Device = device };
      _dbContext.Sensors.Add(sensor);
      await _dbContext.SaveChangesAsync();

      var users = new List<User>();
      await foreach (var (u, r) in _service.ListAllowedUsersBySensorAsync(sensor, 0, 10)) {
        users.Add(u);
      }

      Assert.Empty(users);
    }

    [Fact]
    public async Task ListAllowedUsersBySensorAsync_ThrowsIfOffsetIsNegative()
    {
      var (sensor1, sensor2) = await SeedDefaultCaseAsync();

      var exce = await Record.ExceptionAsync(async () => {
        await foreach (var u in _service.ListAllowedUsersBySensorAsync(sensor1, -1, 10)) {
        }
      });
      Assert.NotNull(exce);
      var details = Assert.IsType<InvalidArgumentException>(exce);
      Assert.Equal("offset", details.ParamName);
      Assert.Contains("negative", details.Message);
    }

    [Fact]
    public async Task ListAllowedUsersBySensorAsync_WorksForZeroOffsetAndLimits()
    {
      var (sensor1, sensor2) = await SeedDefaultCaseAsync();

      var users = new List<User>();
      await foreach (var (u, r) in _service.ListAllowedUsersBySensorAsync(sensor1, 0, 0))
      {
        users.Add(u);
      }

      Assert.Empty(users);
    }

    [Fact]
    public async Task ListAllowedUsersBySensorAsync_ThrowsIfLimitIsNegative()
    {
      var (sensor1, sensor2) = await SeedDefaultCaseAsync();

      Func<Task> act = async () => { 
        await foreach (var s in _service.ListAllowedUsersBySensorAsync(sensor1, 0, -1)) {
        }
      };

      var exce = await Record.ExceptionAsync(act);
      Assert.NotNull(exce);
      var details = Assert.IsType<InvalidArgumentException>(exce);
      Assert.Equal("limit", details.ParamName);
      Assert.Contains("negative", details.Message);
    }

    [Fact]
    public async Task ListAllowedUsersBySensorAsync_LimitsResultSize()
    {
      var (sensor1, sensor2) = await SeedDefaultCaseAsync();

      var users = new List<User>();
      await foreach (var (u, r) in _service.ListAllowedUsersBySensorAsync(sensor1, 0, 1))
      {
        users.Add(u);
      }

      Assert.Single(users);
      Assert.Contains(users, u => u.UserName == "normal-user-1" || u.UserName == "admin-user");
    }

    [Fact]
    public async Task NumberOfAllowedUsersBySensorAsync_ReturnsSizeOfResult()
    {
      var (sensor1, sensor2) = await SeedDefaultCaseAsync();

      var userCount = await _service.NumberOfAllowedUsersBySensorAsync(sensor1);

      Assert.Equal(2 + 1, userCount);
    }

    [Fact]
    public async Task ListSensorsByUserAsync_ReturnsSensors()
    {
      await SeedDefaultCaseAsync();
      var user = await _userManager.FindByNameAsync("normal-user-1");

      var sensors = new List<Sensor>();
      await foreach (var u in _service.ListSensorsByUserAsync(user, 0, 10))
      {
        sensors.Add(u);
      }

      Assert.Equal(2, sensors.Count);
      Assert.Contains(sensors, x => x.Name == "sensor-1");
      Assert.Contains(sensors, x => x.Name == "sensor-2");
    }

    [Fact]
    public async Task ListSensorsByUserAsync_ReturnsEmptyList()
    {
      await SeedDefaultCaseAsync();
      var user = await _userManager.FindByNameAsync("normal-user-3");

      var sensors = new List<Sensor>();
      await foreach (var u in _service.ListSensorsByUserAsync(user, 0, 10))
      {
        sensors.Add(u);
      }

      Assert.Empty(sensors);
    }

    [Fact]
    public async Task ListSensorsByUserAsync_ReturnsEverythingForAdmin()
    {
      await SeedDefaultCaseAsync();
      var admin = await _userManager.FindByNameAsync("admin-user");

      var sensors = new List<Sensor>();
      await foreach (var u in _service.ListSensorsByUserAsync(admin, 0, 10))
      {
        sensors.Add(u);
      }

      Assert.Equal(_dbContext.Sensors.Count(), sensors.Count());
    }

    [Fact]
    public async Task ListSensorsByUserAsync_ThrowsIfOffsetIsNegative()
    {
      await SeedDefaultCaseAsync();
      var user = await _userManager.FindByNameAsync("normal-user-1");

      Func<Task> act = async () => { 
        await foreach (var s in _service.ListSensorsByUserAsync(user, -1, 10)) {
        }
      };

      var exce = await Record.ExceptionAsync(act);
      Assert.NotNull(exce);
      var details = Assert.IsType<InvalidArgumentException>(exce);
      Assert.Equal("offset", details.ParamName);
      Assert.Contains("negative", details.Message);
    }

    [Fact]
    public async Task ListSensorsByUserAsync_WorksForZeroOffsetAndLimits()
    {
      await SeedDefaultCaseAsync();
      var user = await _userManager.FindByNameAsync("normal-user-3");

      var sensors = new List<Sensor>();
      await foreach (var u in _service.ListSensorsByUserAsync(user, 0, 0))
      {
        sensors.Add(u);
      }

      Assert.Empty(sensors);
    }

    [Fact]
    public async Task ListSensorsByUserAsync_ThrowsIfLimitIsNegative()
    {
      await SeedDefaultCaseAsync();
      var user = await _userManager.FindByNameAsync("normal-user-1");

      Func<Task> act = async () => { 
        await foreach (var s in _service.ListSensorsByUserAsync(user, 0, -10)) {
        }
      };

      var exce = await Record.ExceptionAsync(act);
      Assert.NotNull(exce);
      var details = Assert.IsType<InvalidArgumentException>(exce);
      Assert.Equal("limit", details.ParamName);
      Assert.Contains("negative", details.Message);
    }

    [Fact]
    public async Task ListSensorsByUserAsync_LimitsResultSize()
    {
      await SeedDefaultCaseAsync();
      var user = await _userManager.FindByNameAsync("normal-user-1");

      var sensors = new List<Sensor>();
      await foreach (var s in _service.ListSensorsByUserAsync(user, 0, 1))
      {
        sensors.Add(s);
      }

      Assert.Single(sensors);
      Assert.Equal("sensor-1", sensors[0].Name);
    }

    [Fact]
    public async Task NumberOfSensorsByUserAsync_ReturnsSizeOfResult()
    {
      await SeedDefaultCaseAsync();
      var user = await _userManager.FindByNameAsync("normal-user-1");

      var cnt = await _service.NumberOfSensorsByUserAsync(user);

      Assert.Equal(2, cnt);
    }

  }
}
