using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Globalization;
using System.Text;
using NodaTime;
using NodaTime.Testing;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Jobs;
using SMEIoT.Infrastructure.Data;
using SMEIoT.Tests.Shared;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace SMEIoT.Tests.Core
{
  [Collection("Database collection")]
#pragma warning disable CA1063 // Implement IDisposable Correctly
  public class ToggleMqttEntityStatusJobTest : IDisposable
#pragma warning restore CA1063 // Implement IDisposable Correctly
  {
    private readonly ApplicationDbContext _dbContext;
    private readonly FakeClock _clock;
    private Instant _initial;
    private readonly ToggleMqttEntityStatusJob _job;
    private const int TimeOverWindow = 6;
    private const int TimeInWindow = 1;

    public ToggleMqttEntityStatusJobTest()
    {
      _initial = SystemClock.Instance.GetCurrentInstant();
      _clock = new FakeClock(_initial);
      _dbContext = ApplicationDbContextHelper.BuildTestDbContext(_clock);
      _job = new ToggleMqttEntityStatusJob(_clock, _dbContext);
    }

#pragma warning disable CA1063 // Implement IDisposable Correctly
    public void Dispose()
#pragma warning restore CA1063 // Implement IDisposable Correctly
    {
      _dbContext.Database.ExecuteSqlInterpolated($"TRUNCATE devices, sensors CASCADE;");
      _dbContext.Dispose();
    }

    private async Task<IList<Sensor>> SeedEntitiesAsync(int numOfSensor = 1)
    {
      var deviceName = "device-1";
      var device = new Device {
        Name = deviceName,
        NormalizedName = Device.NormalizeName(deviceName),
        AuthenticationType = DeviceAuthenticationType.PreSharedKey,
        PreSharedKey = "1584E92E7BD84D5C2D155D61807929DE89DC2CCC2D0221191D2B68E7A1494A3AAF3C8F7395CB0BEB1CDA1651102E7CBCEE769F292E9FD72A54FD1FBADF7FF802"
      };
      _dbContext.Devices.Add(device);
      
      var sensors = new List<Sensor>();
      for (var x = 0; x < numOfSensor; ++x) {
        var sensorName = $"sensor-{x+1}";
        var sensor = new Sensor {
          Name = sensorName,
          NormalizedName = Sensor.NormalizeName(sensorName),
          Device = device
        };
        sensors.Add(sensor);
        _dbContext.Sensors.Add(sensor);
      }
      await _dbContext.SaveChangesAsync();
      return sensors;
    }

    [Fact]
    public async Task ScanAndToggleMqttEntityConnectedStatus_DoesNotChangeConnectedStatusIfNotConnected()
    {
      var sensors = await SeedEntitiesAsync(1);
      _clock.AdvanceMinutes(TimeOverWindow);

      _job.ScanAndToggleMqttEntityConnectedStatus();

      Assert.False(sensors[0].Connected);
      Assert.False(sensors[0].Device.Connected);
    }


    [Fact]
    public async Task ScanAndToggleMqttEntityConnectedStatus_SwitchSensorToNotConnectedIfOverFiveMinute()
    {
      var sensors = await SeedEntitiesAsync(1);
      var sensor = sensors[0];
      sensor.Connected = true;
      sensor.LastMessageAt = _initial;
      _dbContext.Sensors.Update(sensor);
      await _dbContext.SaveChangesAsync();
      _clock.AdvanceMinutes(TimeOverWindow);

      _job.ScanAndToggleMqttEntityConnectedStatus();

      Assert.False(sensor.Connected);      
    }

    [Fact]
    public async Task ScanAndToggleMqttEntityConnectedStatus_DoesNotSwitchDeviceToNotConnectedIfOneSensorIsActiveInTimedWindow()
    {
      var sensors = await SeedEntitiesAsync(2);
      var sensor = sensors[0];
      sensor.Connected = true;
      sensor.Device.LastMessageAt = sensor.LastMessageAt = _initial;
      sensor.Device.Connected = true;
      _dbContext.Sensors.Update(sensor);
      await _dbContext.SaveChangesAsync();
      _clock.AdvanceMinutes(TimeInWindow);

      _job.ScanAndToggleMqttEntityConnectedStatus();

      Assert.True(sensor.Device.Connected);      
    }

    [Fact]
    public async Task ScanAndToggleMqttEntityConnectedStatus_SwitchDeviceToNotConnectedIfAllSensorsAreInactiveOverTimeWindow()
    {
      var sensors = await SeedEntitiesAsync(2);
      foreach (var s in sensors) {
        s.Device.Connected = s.Connected = true;
        s.Device.LastMessageAt = s.LastMessageAt = _initial;
      }
      await _dbContext.SaveChangesAsync();
      _clock.AdvanceMinutes(TimeOverWindow);

      _job.ScanAndToggleMqttEntityConnectedStatus();

      var sensor = sensors[0];
      var device = sensor.Device;
      Assert.False(sensor.Connected);
      Assert.False(device.Connected);      
    }
  }
}
