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
using NodaTime;
using NodaTime.Testing;

namespace SMEIoT.Tests.Core
{
  [Collection("Database collection")]
#pragma warning disable CA1063 // Implement IDisposable Correctly
  public class DeviceServiceTest : IDisposable
#pragma warning restore CA1063 // Implement IDisposable Correctly
  {
    private readonly ApplicationDbContext _dbContext;
    private readonly MqttIdentifierService _identifierService;
    private readonly DeviceService _service;
    private Instant _initial;

    public DeviceServiceTest()
    {
      _dbContext = ApplicationDbContextHelper.BuildTestDbContext();
      _initial = SystemClock.Instance.GetCurrentInstant();
      _identifierService = new MqttIdentifierService(new FakeClock(_initial));
      _service = new DeviceService(_dbContext, _identifierService);
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
          PreSharedKey = "1584E92E7BD84D5C2D155D61807929DE89DC2CCC2D0221191D2B68E7A1494A3AAF3C8F7395CB0BEB1CDA1651102E7CBCEE769F292E9FD72A54FD1FBADF7FF802"
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
          PreSharedKey = "1584E92E7BD84D5C2D155D61807929DE89DC2CCC2D0221191D2B68E7A1494A3AAF3C8F7395CB0BEB1CDA1651102E7CBCEE769F292E9FD72A54FD1FBADF7FF802"
        };
        _dbContext.Devices.Add(device);

        for (var i = 0; i < 5; ++i)
        {
          var sensorName = $"sensor-{x+1}-{i+1}";
          var sensor = new Sensor {
            Name = sensorName,
            NormalizedName = Sensor.NormalizeName(sensorName),
            Device = device
          };
          _dbContext.Sensors.Add(sensor);
        }
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
          PreSharedKey = ""
        };
      _dbContext.Devices.Add(device);
      await _dbContext.SaveChangesAsync();
      await _identifierService.RegisterDeviceNameAsync("device-1");
      await _identifierService.RegisterSensorNameWithDeviceNameAsync("sensor-1", "device-1");
      return device;
    }

    private async Task<(Device, Sensor)> SeedOneSensorAsync()
    {
      var device = await SeedOneDeviceAsync();
      await _service.CreateSensorByDeviceAndNameAsync(device, "sensor-1");
      var sensor = await _service.GetSensorByDeviceAndNameAsync(device, "sensor-1");
      return (device, sensor);
    }

    [Fact]
    public async Task BootstrapDeviceWithPreSharedKeyAsync_ReturnsPopulatedDevice()
    {
      
      var deviceName = await _service.BootstrapDeviceWithPreSharedKeyAsync("identity", "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa11111111111111");
      
      Assert.NotEmpty(deviceName);
    }

    [Fact]
    public async Task BootstrapDeviceWithPreSharedKeyAsync_PopulatesADevice()
    {
      var key = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa11111111111111";
      
      var deviceName = await _service.BootstrapDeviceWithPreSharedKeyAsync("Name", key);

      var device = await _service.GetDeviceByNameAsync(deviceName);
      Assert.Equal("Name", device.Name);
      Assert.NotEmpty(device.NormalizedName);
      Assert.Equal(key, device.PreSharedKey);
      Assert.Equal(DeviceAuthenticationType.PreSharedKey, device.AuthenticationType);
      Assert.False(device.Connected);
      Assert.Null(device.ConnectedAt);
      Assert.Null(device.LastMessageAt);
    }

    [Fact]
    public async Task BootstrapDeviceWithPreSharedKeyAsync_OkWithMixedCase()
    {
      var key = "aaaFaAaaaa1aaafaab33223cdefa456789aaABCDEFaaaaa23aa11111111111111";
      
      var deviceName = await _service.BootstrapDeviceWithPreSharedKeyAsync("Name", key);

      var device = await _service.GetDeviceByNameAsync(deviceName);
      Assert.Equal("Name", device.Name);
      Assert.NotEmpty(device.NormalizedName);
      Assert.Equal(key, device.PreSharedKey);
      Assert.Equal(DeviceAuthenticationType.PreSharedKey, device.AuthenticationType);
      Assert.False(device.Connected);
      Assert.Null(device.ConnectedAt);
      Assert.Null(device.LastMessageAt);
    }

    [Fact]
    public async Task BootstrapDeviceWithPreSharedKeyAsync_ForbidsSomeName()
    {
      foreach (var name in DeviceService.ForbiddenDeviceNames) {
        Task Act() => _service.BootstrapDeviceWithPreSharedKeyAsync(name, "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa11111111111111");

        var exce = await Record.ExceptionAsync(Act);
        Assert.NotNull(exce);
        var details = Assert.IsType<InvalidArgumentException>(exce);
        Assert.Equal("name", details.ParamName);
      }
    }

    [Fact]
    public async Task BootstrapDeviceWithPreSharedKeyAsync_ThrowsIfNotHex()
    {
      
      Task Act() => _service.BootstrapDeviceWithPreSharedKeyAsync("Name", "is-this-is-not-hex-this-is-not-hex-this-is-not-hex-this-is-not-hex");

      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var details = Assert.IsType<InvalidArgumentException>(exce);
      Assert.Equal("key", details.ParamName);
      Assert.Contains("hex", details.Message);
    }

    [Fact]
    public async Task BootstrapDeviceWithPreSharedKeyAsync_WorksForOneGeneratedKey()
    {
      await _service.BootstrapDeviceWithPreSharedKeyAsync("Name", "1584E92E7BD84D5C2D155D61807929DE89DC2CCC2D0221191D2B68E7A1494A3AAF3C8F7395CB0BEB1CDA1651102E7CBCEE769F292E9FD72A54FD1FBADF7FF802");
    }

    [Fact]
    public async Task BootstrapDeviceWithPreSharedKeyAsync_ThrowsIfKeyTooShort()
    {

      Task Act() => _service.BootstrapDeviceWithPreSharedKeyAsync("Name", new string('1', 63));

      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var details = Assert.IsType<InvalidArgumentException>(exce);
      Assert.Equal("key", details.ParamName);
      Assert.Contains("short", details.Message);
    }

    [Fact]
    public async Task BootstrapDeviceWithPreSharedKeyAsync_ThrowsIfKeyTooLong()
    {

      Task Act() => _service.BootstrapDeviceWithPreSharedKeyAsync("Name", new string('1', 129));

      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var details = Assert.IsType<InvalidArgumentException>(exce);
      Assert.Equal("key", details.ParamName);
      Assert.Contains("long", details.Message);
    }

    [Fact]
    public async Task BootstrapDeviceWithPreSharedKeyAsync_ThrowsIfNameTooShort()
    {

      Task Act() => _service.BootstrapDeviceWithPreSharedKeyAsync("na", "aaaaaaaaaaaaaaaaaa11111111111111");

      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var details = Assert.IsType<InvalidArgumentException>(exce);
      Assert.Equal("name", details.ParamName);
      Assert.Contains("short", details.Message);
    }

    [Fact]
    public async Task BootstrapDeviceWithPreSharedKeyAsync_ThrowsIfNameTooLong()
    {

      Task Act() => _service.BootstrapDeviceWithPreSharedKeyAsync(new string('n', 1001), "aaaaaaaaaaaaaaaaaa11111111111111");

      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var details = Assert.IsType<InvalidArgumentException>(exce);
      Assert.Equal("name", details.ParamName);
      Assert.Contains("long", details.Message);
    }

    [Fact]
    public async Task UpdateDeviceConfigAsync_OkWithMixedCase()
    {
      var device = await SeedOneDeviceAsync();
      var key = "aaaFaAaaaa1aaafaab33223cdefa456789aaABCDEFaaaaa23aa11111111111111";
      
      await _service.UpdateDeviceConfigAsync(device, key);

      var stored = await _service.GetDeviceByNameAsync(device.Name);
      Assert.Equal(key, stored.PreSharedKey);
    }
    
    [Fact]
    public async Task UpdateDeviceConfigAsync_ThrowsIfNotHex()
    {
      var device = await SeedOneDeviceAsync();
      var key = "is-this-is-not-hex-this-is-not-hex-this-is-not-hex-this-is-not-hex";
      
      Task Act() => _service.UpdateDeviceConfigAsync(device, key);

      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var details = Assert.IsType<InvalidArgumentException>(exce);
      Assert.Equal("key", details.ParamName);
      Assert.Contains("hex", details.Message);
    }

    [Fact]
    public async Task UpdateDeviceConfigAsync_ThrowsIfKeyTooShort()
    {
      var device = await SeedOneDeviceAsync();
      var key = new string('1', 63);
      
      Task Act() => _service.UpdateDeviceConfigAsync(device, key);

      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var details = Assert.IsType<InvalidArgumentException>(exce);
      Assert.Equal("key", details.ParamName);
      Assert.Contains("short", details.Message);
    }

    [Fact]
    public async Task UpdateDeviceConfigAsync_ThrowsIfKeyTooLong()
    {
      var device = await SeedOneDeviceAsync();
      var key = new string('1', 129);
      
      Task Act() => _service.UpdateDeviceConfigAsync(device, key);

      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var details = Assert.IsType<InvalidArgumentException>(exce);
      Assert.Equal("key", details.ParamName);
      Assert.Contains("long", details.Message);
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
    public async Task ListSensorsAsync_ReturnsDeviceWithSensor()
    {
      await SeedOneSensorAsync();
      var res = new List<Sensor>();

      await foreach (var d in _service.ListSensorsAsync(0, 10))
      {
        res.Add(d);
      }

      Assert.Single(res);
      Assert.Equal("device-1", res[0].Device.Name);
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
    public async Task CreateSensorByDeviceAndNameAsync_ThrowsWhenSensorExists()
    {
      var device = await SeedOneDeviceAsync();
      await _service.CreateSensorByDeviceAndNameAsync(device, "sensor-1");

      Task Act() => _service.CreateSensorByDeviceAndNameAsync(device, "sensor-1");

      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var exist = Assert.IsType<EntityExistException>(exce);
      Assert.Equal("sensorName", exist.ParamName);
    }

    [Fact]
    public async Task CreateSensorByDeviceAndNameAsync_ThrowsWhenSensorDoesNotSendMessages()
    {
      var device = await SeedOneDeviceAsync();

      Task Act() => _service.CreateSensorByDeviceAndNameAsync(device, "sensor-not-working");

      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var notFound = Assert.IsType<EntityNotFoundException>(exce);
      Assert.Equal("sensorName", notFound.ParamName);
    }

    [Fact]
    public async Task GetSensorByDeviceAndNameAsync_GetsSensor()
    {
      var (device, _) = await SeedOneSensorAsync();

      var fetched = await _service.GetSensorByDeviceAndNameAsync(device, "sensor-1");

      Assert.Equal("sensor-1", fetched.Name);
    }

    [Fact]
    public async Task GetSensorByDeviceAndNameAsync_ThrowsIfSensorCanNotBeFound()
    {
      var (device, _) = await SeedOneSensorAsync();

      Task Act() => _service.GetSensorByDeviceAndNameAsync(device, "sensor-not-exist");

      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var notFound = Assert.IsType<EntityNotFoundException>(exce);
      Assert.Equal("sensorName", notFound.ParamName);
    }

    [Fact]
    public async Task RemoveSensorByDeviceAndNameAsync_ThrowsIfNameDoesNotFound()
    {
      var (device, _) = await SeedOneSensorAsync();

      Task Act() => _service.RemoveSensorByDeviceAndNameAsync(device, "sensor-not-exists");

      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var notFound = Assert.IsType<EntityNotFoundException>(exce);
      Assert.Equal("sensorName", notFound.ParamName);
    }

    [Fact]
    public async Task RemoveSensorByNameAsync_RemovesSensor()
    {
      var (device, _) = await SeedOneSensorAsync();

      await _service.RemoveSensorByDeviceAndNameAsync(device, "sensor-1");

      Assert.Empty(_dbContext.Sensors);
    }

    [Fact]
    public async Task RemoveDeviceByNameAsync_RemovesDevice()
    {
      await SeedOneDeviceAsync();

      await _service.RemoveDeviceByNameAsync("device-1");

      Assert.Empty(_dbContext.Devices);
    }

    [Fact]
    public async Task RemoveDeviceByNameAsync_ThrowsIfNameDoesNotFound()
    {
      await SeedOneDeviceAsync();

      Task Act() =>  _service.RemoveDeviceByNameAsync("device-not-exists");

      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var notFound = Assert.IsType<EntityNotFoundException>(exce);
      Assert.Equal("deviceName", notFound.ParamName);
    }

    [Fact]
    public async Task UpdateDeviceTimestampsAndStatusAsync_UpdatesEverythingWithFirstInstant()
    {
      var device = await SeedOneDeviceAsync();

      await _service.UpdateDeviceTimestampsAndStatusAsync(device, _initial);

      Assert.True(device.Connected);
      Assert.Equal(_initial, device.ConnectedAt);
      Assert.Equal(_initial, device.LastMessageAt);
    }

    [Fact]
    public async Task UpdateDeviceTimestampsAndStatusAsync_UpdatesConnectedAndLastMessageTimestampWithSecondInstant()
    {
      var device = await SeedOneDeviceAsync();
      var future = _initial + Duration.FromDays(1);
      await _service.UpdateDeviceTimestampsAndStatusAsync(device, _initial);
      device.Connected = false;
      _dbContext.Devices.Update(device);
      await _dbContext.SaveChangesAsync();

      await _service.UpdateDeviceTimestampsAndStatusAsync(device, future);

      Assert.True(device.Connected);
      Assert.Equal(_initial, device.ConnectedAt);
      Assert.Equal(future, device.LastMessageAt);
    }


    [Fact]
    public async Task UpdateDeviceTimestampsAndStatusAsync_RespectsGracePeriodWithNoUpdate()
    {
      var device = await SeedOneDeviceAsync();
      var future1 = _initial + Duration.FromSeconds(1);
      await _service.UpdateDeviceTimestampsAndStatusAsync(device, _initial);
      device.Connected = false;
      _dbContext.Devices.Update(device);
      await _dbContext.SaveChangesAsync();

      await _service.UpdateDeviceTimestampsAndStatusAsync(device, future1);

      Assert.True(device.Connected);
      Assert.Equal(_initial, device.ConnectedAt);
      Assert.Equal(_initial, device.LastMessageAt);
    }

    [Fact]
    public async Task UpdateDeviceTimestampsAndStatusAsync_RespectsGracePeriodWithUpdate()
    {
      var device = await SeedOneDeviceAsync();
      var future = _initial + Duration.FromSeconds(DeviceService.LastMessageAtTimestampUpdateGracePeriod);
      await _service.UpdateDeviceTimestampsAndStatusAsync(device, _initial);
      device.Connected = false;
      _dbContext.Devices.Update(device);
      await _dbContext.SaveChangesAsync();

      await _service.UpdateDeviceTimestampsAndStatusAsync(device, future);

      Assert.True(device.Connected);
      Assert.Equal(_initial, device.ConnectedAt);
      Assert.Equal(future, device.LastMessageAt);
    }

    [Fact]
    public async Task UpdateSensorAndDeviceTimestampsAndStatusAsync_UpdatesEverythingWithFirstMessage()
    {
      var (device, sensor) = await SeedOneSensorAsync();

      await _service.UpdateSensorAndDeviceTimestampsAndStatusAsync(sensor, _initial);

      Assert.True(sensor.Connected);
      Assert.Equal(_initial, sensor.ConnectedAt);
      Assert.Equal(_initial, sensor.LastMessageAt);
      Assert.True(device.Connected);
      Assert.Equal(_initial, device.ConnectedAt);
      Assert.Equal(_initial, device.LastMessageAt);
    }

    [Fact]
    public async Task UpdateSensorAndDeviceTimestampsAndStatusAsync_UpdatesDeviceWithSecondMessage()
    {
      var (device, sensor) = await SeedOneSensorAsync();
      var future = _initial + Duration.FromDays(1);
      await _service.UpdateSensorAndDeviceTimestampsAndStatusAsync(sensor, _initial);
      sensor.Connected = false;
      sensor.Device.Connected = false;
      _dbContext.Sensors.Update(sensor);
      await _dbContext.SaveChangesAsync();

      await _service.UpdateSensorAndDeviceTimestampsAndStatusAsync(sensor, future);

      Assert.True(sensor.Connected);
      Assert.Equal(_initial, sensor.ConnectedAt);
      Assert.Equal(future, sensor.LastMessageAt);
      Assert.True(device.Connected);
      Assert.Equal(_initial, device.ConnectedAt);
      Assert.Equal(future, device.LastMessageAt);
    }

    [Fact]
    public async Task UpdateSensorAndDeviceTimestampsAndStatusAsync_RespectsGracePeriodWithNoUpdate()
    {
      var (device, sensor) = await SeedOneSensorAsync();
      var future1 = _initial + Duration.FromSeconds(1);
      await _service.UpdateSensorAndDeviceTimestampsAndStatusAsync(sensor, _initial);
      sensor.Connected = false;
      sensor.Device.Connected = false;
      _dbContext.Sensors.Update(sensor);
      await _dbContext.SaveChangesAsync();

      await _service.UpdateSensorAndDeviceTimestampsAndStatusAsync(sensor, future1);

      Assert.True(sensor.Connected);
      Assert.Equal(_initial, sensor.ConnectedAt);
      Assert.Equal(_initial, sensor.LastMessageAt);
      Assert.True(device.Connected);
      Assert.Equal(_initial, device.ConnectedAt);
      Assert.Equal(_initial, device.LastMessageAt);
    }

    [Fact]
    public async Task UpdateSensorAndDeviceTimestampsAndStatusAsync_RespectsGracePeriodWithUpdate()
    {
      var (device, sensor) = await SeedOneSensorAsync();
      var future = _initial + Duration.FromSeconds(DeviceService.LastMessageAtTimestampUpdateGracePeriod);
      await _service.UpdateSensorAndDeviceTimestampsAndStatusAsync(sensor, _initial);
      sensor.Connected = false;
      sensor.Device.Connected = false;
      _dbContext.Sensors.Update(sensor);
      await _dbContext.SaveChangesAsync();

      await _service.UpdateSensorAndDeviceTimestampsAndStatusAsync(sensor, future);

      Assert.True(sensor.Connected);
      Assert.Equal(_initial, sensor.ConnectedAt);
      Assert.Equal(future, sensor.LastMessageAt);
      Assert.True(device.Connected);
      Assert.Equal(_initial, device.ConnectedAt);
      Assert.Equal(future, device.LastMessageAt);
    }
  }
}
