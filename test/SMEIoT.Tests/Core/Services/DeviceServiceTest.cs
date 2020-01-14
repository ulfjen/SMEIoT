using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Exceptions;
using SMEIoT.Core.Services;
using SMEIoT.Infrastructure.Data;
using SMEIoT.Tests.Shared;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace SMEIoT.Tests.Core.Services
{
#pragma warning disable CA1063 // Implement IDisposable Correctly
  public class DeviceServiceTest : IDisposable
#pragma warning restore CA1063 // Implement IDisposable Correctly
  {
    private readonly ApplicationDbContext _dbContext;
    private readonly DeviceService _service;

    public DeviceServiceTest()
    {
      _dbContext = ApplicationDbContextHelper.BuildTestDbContext();
      _service = new DeviceService(_dbContext);
    }

#pragma warning disable CA1063 // Implement IDisposable Correctly
    public void Dispose()
#pragma warning restore CA1063 // Implement IDisposable Correctly
    {
      _dbContext.Database.ExecuteSqlInterpolated($"TRUNCATE devices, sensors CASCADE;");
      _dbContext.Dispose();
    }

    private async Task SeedDefaultDevicesAsync()
    {
      for (var x = 0; x < 15; ++x)
      {
        var deviceName = $"device-{x+1}";
        var device = new Device {
          Name = deviceName,
          NormalizedName = Device.NormalizeName(deviceName),
          AuthenticationType = DeviceAuthenticationType.PreSharedKey,
          PreSharedKey = "key"
        };
        _dbContext.Add(device);
      }
      await _dbContext.SaveChangesAsync();
    }

    private async Task SeedDefaultSensorsAsync()
    {
      for (var x = 0; x < 3; ++x)
      {
        var deviceName = $"device-{x+1}";
        var device = new Device {
          Name = deviceName,
          NormalizedName = Device.NormalizeName(deviceName),
          AuthenticationType = DeviceAuthenticationType.PreSharedKey,
          PreSharedKey = "key"
        };
        for (var i = 0; i < 5; ++i)
        {
          var sensorName = $"sensor-{x+1}-{i+1}";
          var sensor = new Sensor {
            Name = sensorName,
            NormalizedName = Sensor.NormalizeName(sensorName),
            Device = device
          };
          _dbContext.Add(sensor);
        }
        _dbContext.Add(device);
      }
      await _dbContext.SaveChangesAsync();
    }

    private async Task<Device> SeedOneDeviceAsync()
    {
      var deviceName = "device-1";
      var device = new Device {
          Name = deviceName,
          NormalizedName = Device.NormalizeName(deviceName),
          AuthenticationType = DeviceAuthenticationType.PreSharedKey,
          PreSharedKey = "key"
        };
      _dbContext.Add(device);
      await _dbContext.SaveChangesAsync();
      return device;
    }

    [Fact]
    public async Task BootstrapDeviceWithPreSharedKeyAsync_ReturnsPopulatedDevice()
    {
      // arrange
      
      // act
      var deviceName = await _service.BootstrapDeviceWithPreSharedKeyAsync("identity", "key");
      
      // assert
      Assert.NotEmpty(deviceName);
    }

    [Fact]
    public async Task BootstrapDeviceWithPreSharedKeyAsync_PopulatesADevice()
    {
      
      var deviceName = await _service.BootstrapDeviceWithPreSharedKeyAsync("Name", "key");

      var device = await _service.GetDeviceByNameAsync(deviceName);
      Assert.Equal("Name", device.Name);
      Assert.NotEmpty(device.NormalizedName);
      Assert.Equal("key", device.PreSharedKey);
      Assert.Equal(DeviceAuthenticationType.PreSharedKey, device.AuthenticationType);
      Assert.False(device.Connected);
      Assert.Null(device.ConnectedAt);
      Assert.Null(device.LastMessageAt);
    }
    
    [Fact]
    public async Task GetDeviceByNameAsync_ThrowsIfNoDevice()
    {
      
      Task Act() => _service.GetDeviceByNameAsync("not-exist-device");

      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var notFound = Assert.IsType<EntityNotFoundException>(exce);
      Assert.Equal("deviceName", notFound.ParamName);
    }
    
    [Fact]
    public async Task GetDeviceByNameAsync_ReturnsDevice()
    {
      const string name = "named-device";
      _dbContext.Devices.Add(new Device{Name=name, NormalizedName = Device.NormalizeName(name)});
      await _dbContext.SaveChangesAsync();

      var device = await _service.GetDeviceByNameAsync(name);
      
      Assert.Equal(name, device.Name); 
    }

    [Fact]
    public async Task GetDeviceByNameAsync_IgnoresCase()
    {
      const string name = "named-device";
      _dbContext.Devices.Add(new Device{Name=name, NormalizedName = Device.NormalizeName(name)});
      await _dbContext.SaveChangesAsync();

      var device = await _service.GetDeviceByNameAsync("NAMED-device");
      
      Assert.Equal(name, device.Name); 
    }

    [Fact]
    public async Task GetARandomUnconnectedDeviceAsync_ReturnsNull()
    {

      var device = await _service.GetARandomUnconnectedDeviceAsync();
      
      Assert.Null(device); 
    }

    [Fact]
    public async Task GetARandomUnconnectedDeviceAsync_ReturnsDevice()
    {
      const string name = "named-device";
      _dbContext.Devices.Add(new Device{Name=name, NormalizedName = Device.NormalizeName(name)});
      await _dbContext.SaveChangesAsync();

      var device = await _service.GetARandomUnconnectedDeviceAsync();
      
      Assert.Equal(name, device!.Name); 
    }

    [Fact]
    public async Task ListDevicesAsync_ThrowsNegativeOffset()
    {
      await SeedDefaultDevicesAsync();
      
      var details = await Assert.ThrowsAsync<InvalidArgumentException>(async () =>
      {
        await foreach (var d in _service.ListDevicesAsync(-1, 20))
        {
        }
      });
      Assert.Equal("offset", details.ParamName);
    }

    [Fact]
    public async Task ListDevicesAsync_AllowZeroOffset()
    {
      await SeedDefaultDevicesAsync();
      var res = new List<Device>();

      await foreach (var d in _service.ListDevicesAsync(0, 12))
      {
        res.Add(d);
      }

      Assert.Equal(12, res.Count);
    }
    
    [Fact]
    public async Task ListDevicesAsync_ThrowsNegativeLimit()
    {
      await SeedDefaultDevicesAsync();
      
      var details = await Assert.ThrowsAsync<InvalidArgumentException>(async () =>
      {
        await foreach (var d in _service.ListDevicesAsync(1, -1))
        {
        }
      });
      Assert.Equal("limit", details.ParamName);
    }

    [Fact]
    public async Task ListDevicesAsync_AllowZeroLimit()
    {
      await SeedDefaultDevicesAsync();
      var res = new List<Device>();

      await foreach (var d in _service.ListDevicesAsync(0, 0))
      {
        res.Add(d);
      }

      Assert.Empty(res);
    }
    
    [Fact]
    public async Task ListDevicesAsync_ReturnsDevices()
    {
      await SeedDefaultDevicesAsync();
      var res = new List<Device>();

      await foreach (var d in _service.ListDevicesAsync(2, 10))
      {
        res.Add(d);
      }

      Assert.Equal(10, res.Count);
      Assert.Contains(res, d => d.Name == "device-3");
    }

    [Fact]
    public async Task ListSensorsAsync_ThrowsNegativeOffset()
    {
      await SeedDefaultSensorsAsync();
      
      var details = await Assert.ThrowsAsync<InvalidArgumentException>(async () =>
      {
        await foreach (var d in _service.ListSensorsAsync(-1, 20))
        {
        }
      });
      Assert.Equal("offset", details.ParamName);
    }

    [Fact]
    public async Task ListSensorsAsync_AllowZeroOffset()
    {
      await SeedDefaultSensorsAsync();
      var res = new List<Sensor>();

      await foreach (var d in _service.ListSensorsAsync(0, 12))
      {
        res.Add(d);
      }

      Assert.Equal(12, res.Count);
    }
    
    [Fact]
    public async Task ListSensorsAsync_ThrowsNegativeLimit()
    {
      await SeedDefaultSensorsAsync();
      
      var details = await Assert.ThrowsAsync<InvalidArgumentException>(async () =>
      {
        await foreach (var d in _service.ListSensorsAsync(1, -1))
        {
        }
      });
      Assert.Equal("limit", details.ParamName);
    }

    [Fact]
    public async Task ListSensorsAsync_AllowZeroLimit()
    {
      await SeedDefaultSensorsAsync();
      var res = new List<Sensor>();

      await foreach (var d in _service.ListSensorsAsync(0, 0))
      {
        res.Add(d);
      }

      Assert.Empty(res);
    }
    
    [Fact]
    public async Task ListSensorsAsync_ReturnsSensors()
    {
      await SeedDefaultSensorsAsync();
      var res = new List<Sensor>();

      await foreach (var d in _service.ListSensorsAsync(2, 10))
      {
        res.Add(d);
      }

      Assert.Equal(10, res.Count);
      Assert.Contains(res, d => d.Name == "sensor-1-3");
    }

    [Fact]
    public async Task ListSensorsByDeviceAsync_ReturnsSensors()
    {
      await SeedDefaultSensorsAsync();
      var device = await _dbContext.Devices.FirstOrDefaultAsync(d => d.Name == "device-1");
      var res = new List<Sensor>();

      await foreach (var d in _service.ListSensorsByDeviceAsync(device))
      {
        res.Add(d);
      }

      Assert.Equal(5, res.Count);
      Assert.Contains(res, d => d.Name == "sensor-1-3");
    }

    [Fact]
    public async Task CreateSensorByDeviceAndNameAsync_CreatesOneSensor()
    {
      var device = await SeedOneDeviceAsync();

      await _service.CreateSensorByDeviceAndNameAsync(device, "sensor-1");

      Assert.Single(_dbContext.Sensors.ToList());
    }

    [Fact]
    public async Task GetSensorByDeviceAndNameAsync_GetsSensor()
    {
      var device = await SeedOneDeviceAsync();
      await _service.CreateSensorByDeviceAndNameAsync(device, "sensor-1");

      var fetched = await _service.GetSensorByDeviceAndNameAsync(device, "sensor-1");

      Assert.Equal("sensor-1", fetched.Name);
    }
  }
}
