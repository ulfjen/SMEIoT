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
  public class SensorValueServiceTest : IDisposable
#pragma warning restore CA1063 // Implement IDisposable Correctly
  {
    private readonly ApplicationDbContext _dbContext;
    private readonly MqttIdentifierService _identifierService;
    private readonly DeviceService _deviceService;
    private readonly SensorValueService _service;
    private Instant _initial;

    public SensorValueServiceTest()
    {
      _dbContext = ApplicationDbContextHelper.BuildTestDbContext();
      _initial = SystemClock.Instance.GetCurrentInstant();
      _identifierService = new MqttIdentifierService(new FakeClock(_initial));
      _deviceService = new DeviceService(_dbContext, _identifierService);
      _service = new SensorValueService(_dbContext);
    }

#pragma warning disable CA1063 // Implement IDisposable Correctly
    public void Dispose()
#pragma warning restore CA1063 // Implement IDisposable Correctly
    {
      _dbContext.Database.ExecuteSqlInterpolated($"TRUNCATE devices, sensors, sensor_values RESTART IDENTITY CASCADE;");
      _dbContext.Dispose();
    }

    private async Task SeedDefaultSensorsAsync()
    {
      for (var x = 0; x < 2; ++x)
      {
        var deviceName = $"device-{x+1}";
        var device = new Device {
          Name = deviceName,
          NormalizedName = Device.NormalizeName(deviceName),
          AuthenticationType = DeviceAuthenticationType.PreSharedKey,
          PreSharedKey = "1584E92E7BD84D5C2D155D61807929DE89DC2CCC2D0221191D2B68E7A1494A3AAF3C8F7395CB0BEB1CDA1651102E7CBCEE769F292E9FD72A54FD1FBADF7FF802"
        };
        _dbContext.Devices.Add(device);

        for (var i = 0; i < 3; ++i)
        {
          var sensorName = $"sensor-{x+1}-{i+1}";
          var sensor = new Sensor {
            Name = sensorName,
            NormalizedName = Sensor.NormalizeName(sensorName),
            Device = device
          };
          _dbContext.Sensors.Add(sensor);

          for (var j = 1; j <= 15; ++j) {
            _dbContext.SensorValues.Add(new SensorValue {
              Sensor = sensor,
              Value = x* 100.0 + j * 1.0,
              CreatedAt = _initial + Duration.FromDays(x) + Duration.FromSeconds(j)
            });
          }
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
      await _deviceService.CreateSensorByDeviceAndNameAsync(device, "sensor-1");
      var sensor = await _deviceService.GetSensorByDeviceAndNameAsync(device, "sensor-1");
      for (var i = 1; i <= 15; ++i) {
        _dbContext.SensorValues.Add(new SensorValue {
          Sensor = sensor,
          Value = i * 1.0,
          CreatedAt = _initial + Duration.FromSeconds(i)
        });
      }
      await _dbContext.SaveChangesAsync();

      return (device, sensor);
    }

    [Fact]
    public async Task AddSensorValueAsync_CreatesValue()
    {
      var (device, sensor) = await SeedOneSensorAsync();
      var time = _initial + Duration.FromDays(10);
      
      await _service.AddSensorValueAsync(sensor, 2.0, time);

      Assert.Equal(15 + 1, await _dbContext.SensorValues.CountAsync());
      var value = await _dbContext.SensorValues.OrderByDescending(sv => sv.CreatedAt).FirstOrDefaultAsync();
      var eps = 1e-7;
      Assert.True(value.Value > 2.0 - eps && value.Value < 2.0+eps);
      Assert.Equal(time.ToString(), value.CreatedAt.ToString());
      Assert.Equal(device.Id, value.Sensor.DeviceId);
    }

    [Fact]
    public async Task GetNumberTimeSeriesBySensorAsync_ReturnsNothingIfAsksFuture()
    {
      var (device, sensor) = await SeedOneSensorAsync();
      var items = new List<(double value, Instant createdAt)>();

      await foreach (var n in _service.GetNumberTimeSeriesBySensorAsync(sensor, _initial+Duration.FromSeconds(16), Duration.FromSeconds(5))) {
        items.Add((n.value, n.createdAt));
      }

      Assert.Empty(items);
    }

    [Fact]
    public async Task GetNumberTimeSeriesBySensorAsync_ReturnsNothingIfAsksPast()
    {
      var (device, sensor) = await SeedOneSensorAsync();
      var items = new List<(double value, Instant createdAt)>();

      await foreach (var n in _service.GetNumberTimeSeriesBySensorAsync(sensor, _initial-Duration.FromSeconds(5), Duration.FromSeconds(5))) {
        items.Add((n.value, n.createdAt));
      }

      Assert.Empty(items);
    }

    [Fact]
    public async Task GetNumberTimeSeriesBySensorAsync_ReturnsNothingIfNotInDuration()
    {
      var (device, sensor) = await SeedOneSensorAsync();
      var items = new List<(double value, Instant createdAt)>();

      await foreach (var n in _service.GetNumberTimeSeriesBySensorAsync(sensor, _initial, Duration.FromMilliseconds(5))) {
        items.Add((n.value, n.createdAt));
      }

      Assert.Empty(items);
    }

    [Fact]
    public async Task GetNumberTimeSeriesBySensorAsync_ReturnsValueInDuration()
    {
      var (device, sensor) = await SeedOneSensorAsync();
      var items = new List<(double value, Instant createdAt)>();

      await foreach (var n in _service.GetNumberTimeSeriesBySensorAsync(sensor, _initial, Duration.FromSeconds(10))) {
        items.Add((n.value, n.createdAt));
      }

      Assert.Equal(9, items.Count);
      foreach (var i in items) {
        Assert.True(i.createdAt < _initial + Duration.FromSeconds(10));
      }
    }

    [Fact]
    public async Task GetNumberTimeSeriesBySensorAsync_ReturnsValuesWithCreatedOrder()
    {
      var (device, sensor) = await SeedOneSensorAsync();

      var items = new List<(double value, Instant createdAt)>();
      await foreach (var n in _service.GetNumberTimeSeriesBySensorAsync(sensor, _initial, Duration.FromSeconds(10))) {
        items.Add((n.value, n.createdAt));
      }

      Instant last = Instant.MinValue;
      foreach (var i in items) {
        Assert.True(i.createdAt > last);
        last = i.createdAt;
      }
    }

    [Fact]
    public async Task GetLastNumberOfValuesBySensorsAsync_ThrowsIfCountIsNegative()
    {

      var details = await Assert.ThrowsAsync<InvalidArgumentException>(async () =>
      {
        await foreach (var v in _service.GetLastNumberOfValuesBySensorsAsync(new Sensor[]{}, -1))
        {
        }
      });

      Assert.Equal("count", details.ParamName);
      Assert.Contains("negative", details.Message);
    }

    [Fact]
    public async Task GetLastNumberOfValuesBySensorsAsync_ReturnsEmptyIfNoSensor()
    {
      await SeedDefaultSensorsAsync();
      var items = new List<(Sensor sensor, double value, Instant createdAt)>();

      await foreach (var i in _service.GetLastNumberOfValuesBySensorsAsync(new Sensor[]{}, 1)) {
        items.Add(i);
      }

      Assert.Empty(items);
    }

    [Fact]
    public async Task GetLastNumberOfValuesBySensorsAsync_GetsValuesIfAvailable()
    {
      await SeedDefaultSensorsAsync();
      var items = new List<(Sensor sensor, double value, Instant createdAt)>();
      var sensor1 = await _dbContext.Sensors.Where(s => s.Name == "sensor-1-1").FirstOrDefaultAsync();
      var sensor2 = await _dbContext.Sensors.Where(s => s.Name == "sensor-2-2").FirstOrDefaultAsync();

      await foreach (var i in _service.GetLastNumberOfValuesBySensorsAsync(new []{sensor1, sensor2}, 5)) {
        items.Add(i);
      }

      Assert.Equal(10, items.Count);
      foreach (var i in items) {
        Assert.True(i.sensor == sensor1 || i.sensor == sensor2);
        if (i.sensor == sensor1) {
          Assert.True(i.createdAt >= _initial + Duration.FromDays(0) + Duration.FromSeconds(10));
        } else {
          Assert.True(i.createdAt >= _initial + Duration.FromDays(1) + Duration.FromSeconds(10));
        }
      }
    }

    [Fact]
    public async Task GetLastNumberOfValuesBySensorsAsync_ReturnsValuesWithCreatedOrder()
    {
      await SeedDefaultSensorsAsync();
      
      var items = new List<(Sensor sensor, double value, Instant createdAt)>();
      var sensor1 = await _dbContext.Sensors.Where(s => s.Name == "sensor-1-1").FirstOrDefaultAsync();
      var sensor2 = await _dbContext.Sensors.Where(s => s.Name == "sensor-2-2").FirstOrDefaultAsync();

      await foreach (var i in _service.GetLastNumberOfValuesBySensorsAsync(new []{sensor1, sensor2}, 5)) {
        items.Add(i);
      }

      Instant min1 = Instant.MinValue;
      Instant min2 = Instant.MinValue;
      foreach (var i in items) {
        Assert.True(i.sensor == sensor1 || i.sensor == sensor2);
        if (i.sensor == sensor1) {
          Assert.True(min1 < i.createdAt);
          min1 = i.createdAt;
        } else {
          Assert.True(min2 < i.createdAt);
          min2 = i.createdAt;
        }
      }
    }
  }
}
