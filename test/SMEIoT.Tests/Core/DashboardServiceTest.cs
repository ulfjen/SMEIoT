using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using NodaTime;
using NodaTime.Testing;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Exceptions;
using SMEIoT.Core.Services;
using SMEIoT.Web.Services;
using SMEIoT.Infrastructure.Data;
using SMEIoT.Tests.Shared;
using Xunit;

namespace SMEIoT.Tests.Core
{
  [Collection("Database collection")]
#pragma warning disable CA1063 // Implement IDisposable Correctly
  public class DashboardServiceTest : IDisposable
#pragma warning restore CA1063 // Implement IDisposable Correctly
  {
    private ApplicationDbContext _dbContext;
    private readonly DashboardService _service;
    private readonly UserManager _userManager;
    private Instant _initial;
    private readonly RoleManager<IdentityRole<long>> _roleManager;

    public DashboardServiceTest()
    {
      _initial = SystemClock.Instance.GetCurrentInstant();
      _dbContext = ApplicationDbContextHelper.BuildTestDbContext();
      _userManager = IdentityHelpers.BuildUserManager(ApplicationDbContextHelper.BuildTestDbContext());
      _roleManager = IdentityHelpers.BuildRoleManager(ApplicationDbContextHelper.BuildTestDbContext());
      _service = new DashboardService(_dbContext, _userManager);
    }

#pragma warning disable CA1063 // Implement IDisposable Correctly
    public void Dispose()
#pragma warning restore CA1063 // Implement IDisposable Correctly
    {
      _dbContext.Database.ExecuteSqlInterpolated($"TRUNCATE devices, sensors, user_roles, user_tokens, user_logins, user_claims, role_claims, roles, users RESTART IDENTITY CASCADE;");
      _userManager.Dispose();
      _roleManager.Dispose();
      _dbContext.Dispose();
    }

    private async Task SeedDefaultSystemAsync()
    {
      await _roleManager.CreateAsync(new IdentityRole<long>("Admin"));
      for (var x = 0; x < 10; ++x)
      {
        var userName = $"normal-user-{x+1}";
        var user = new User {UserName = userName, ConcurrencyStamp = Guid.NewGuid().ToString()};
        await _userManager.CreateAsync(user,"a-password-1");
        await _userManager.AddToRolesAsync(await _userManager.FindByNameAsync(userName), new string[] {}); 
      }
      for (var x = 0; x < 5; ++x)
      {
        var userName = $"user-{x+101}";
        var user = new User {UserName = userName, ConcurrencyStamp = Guid.NewGuid().ToString()};
        await _userManager.CreateAsync(user,"a-password-1");
        await _userManager.AddToRoleAsync(await _userManager.FindByNameAsync(userName), "admin"); 
      }
      for (var x = 0; x < 3; ++x)
      {
        var deviceName = $"device-{x+1}";
        var device = new Device {
          Name = deviceName,
          NormalizedName = Device.NormalizeName(deviceName),
          AuthenticationType = DeviceAuthenticationType.PreSharedKey,
          Connected = x == 2,
          ConnectedAt = _initial,
          LastMessageAt = _initial,
          PreSharedKey = "1584E92E7BD84D5C2D155D61807929DE89DC2CCC2D0221191D2B68E7A1494A3AAF3C8F7395CB0BEB1CDA1651102E7CBCEE769F292E9FD72A54FD1FBADF7FF802"
        };
        _dbContext.Devices.Add(device);

        for (var i = 0; i < 5; ++i)
        {
          var sensorName = $"sensor-{x+1}-{i+1}";
          var sensor = new Sensor {
            Name = sensorName,
            NormalizedName = Sensor.NormalizeName(sensorName),
            Device = device,
            Connected = device.Connected && i >1,
            ConnectedAt = _initial,
            LastMessageAt = _initial,
          };
          _dbContext.Sensors.Add(sensor);
        }
      }
      await _dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task GetSystemHighlightsAsync_ReturnsInformation()
    {
      // arrange
      await SeedDefaultSystemAsync();

      // act
      var highlights = await _service.GetSystemHighlightsAsync();

      // assert
      Assert.Equal(15, highlights.UserCount);
      Assert.Equal(5, highlights.AdminCount);
      Assert.Equal(3, highlights.ConnectedSensorCount);
      Assert.Equal(15, highlights.SensorCount);
      Assert.Equal(1, highlights.ConnectedDeviceCount);
      Assert.Equal(3, highlights.DeviceCount);
    }

    [Fact]
    public async Task GetSystemHighlightsAsync_ReturnsEmpty()
    {

      var highlights = await _service.GetSystemHighlightsAsync();

      Assert.Equal(0, highlights.UserCount);
      Assert.Equal(0, highlights.UserCount);
      Assert.Equal(0, highlights.ConnectedSensorCount);
      Assert.Equal(0, highlights.SensorCount);
      Assert.Equal(0, highlights.ConnectedDeviceCount);
      Assert.Equal(0, highlights.DeviceCount);
    }
  }
}
