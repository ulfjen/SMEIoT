using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using NodaTime.Testing;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Exceptions;
using SMEIoT.Core.Services;
using SMEIoT.Tests.Shared;
using SMEIoT.Infrastructure.Data;
using SMEIoT.Core.Interfaces;
using Moq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace SMEIoT.Tests.Core.Services
{
  [Collection("Database collection")]
  public class MqttMessageIngestionServiceTest : IDisposable
  {
    private readonly Instant _initial;
    private readonly IClock _clock;
    private readonly ApplicationDbContext _dbContext;
    private readonly MqttMessageIngestionService _service;
    private readonly IDeviceService _deviceService;
    private readonly IMqttIdentifierService _mqttIdentifierService;
    private readonly IMosquittoBrokerService _brokerService;
    private readonly ISensorValueService _valueService;

    public MqttMessageIngestionServiceTest()
    {
      _dbContext = ApplicationDbContextHelper.BuildTestDbContext();
      _initial = SystemClock.Instance.GetCurrentInstant();
      _clock = new FakeClock(_initial);

      var mock = new Mock<IMosquittoBrokerPidAccessor>();
      mock.SetupGet(a => a.BrokerPid).Returns(10000);
      var mockPlugin = new Mock<IMosquittoBrokerPluginPidService>();
      mockPlugin.SetupGet(a => a.BrokerPidFromAuthPlugin).Returns(10000);
      var configMock = new Mock<IMqttClientConfigService>();
      configMock.Setup(c => c.GetHost()).Returns("127.0.0.1");
      configMock.Setup(c => c.GetPort()).Returns(12345);

      _brokerService = new MosquittoBrokerService(_clock, new NullLogger<MosquittoBrokerService>(), mock.Object, mockPlugin.Object, configMock.Object);

      _mqttIdentifierService = new MqttIdentifierService(_clock);
      _deviceService = new DeviceService(_dbContext, _mqttIdentifierService);
      _valueService = new SensorValueService(_dbContext);

      _service = new MqttMessageIngestionService(_deviceService, _mqttIdentifierService, _brokerService, _valueService, new NullLogger<MqttMessageIngestionService>());
    }

#pragma warning disable CA1063 // Implement IDisposable Correctly
    public void Dispose()
#pragma warning restore CA1063 // Implement IDisposable Correctly
    {
      _dbContext.Database.ExecuteSqlInterpolated($"TRUNCATE devices, sensors, sensor_values CASCADE;");
      _dbContext.Dispose();
    }

    private async Task SeedDefaultSensor()
    {
      var deviceName = "device-alpha";
      var device = new Device {
        Name = deviceName,
        NormalizedName = Device.NormalizeName(deviceName),
        AuthenticationType = DeviceAuthenticationType.PreSharedKey,
        PreSharedKey = "1584E92E7BD84D5C2D155D61807929DE89DC2CCC2D0221191D2B68E7A1494A3AAF3C8F7395CB0BEB1CDA1651102E7CBCEE769F292E9FD72A54FD1FBADF7FF802"
      };
      _dbContext.Devices.Add(device);

      var sensorName = $"sensor-beta";
      var sensor = new Sensor {
        Name = sensorName,
        NormalizedName = Sensor.NormalizeName(sensorName),
        Device = device
      };
      _dbContext.Sensors.Add(sensor);
      await _dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task ProcessBrokerMessageAsync_DoNothingWhenNotWithBrokerTopicPrefix()
    {
      var message = new MqttMessage("SYS/broker/clients/connected", "120", _clock.GetCurrentInstant());
      var mock = new Mock<IMosquittoBrokerService>();
      mock.Setup(b => b.RegisterBrokerStatisticsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Instant>())).Returns(Task.FromResult(true));
      var service = new MqttMessageIngestionService(_deviceService, _mqttIdentifierService, _brokerService, _valueService, new NullLogger<MqttMessageIngestionService>());

      await service.ProcessBrokerMessageAsync(message);

      mock.Verify(b => b.RegisterBrokerStatisticsAsync("clients/connected", "120", _initial), Times.Never());
    }

    [Fact]
    public async Task ProcessBrokerMessageAsync_SavesValue()
    {
      var message = new MqttMessage("$SYS/broker/clients/connected", "120", _clock.GetCurrentInstant());

      await _service.ProcessBrokerMessageAsync(message);

      var result = await _brokerService.GetBrokerStatisticsAsync("clients/connected");
      Assert.Equal("120", result);
    }

    [Fact]
    public async Task ProcessBrokerMessageAsync_StripsSeconds()
    {
      var message = new MqttMessage("$SYS/broker/uptime", "200 seconds", _clock.GetCurrentInstant());

      await _service.ProcessBrokerMessageAsync(message);

      var result = await _brokerService.GetBrokerStatisticsAsync("uptime");
      Assert.Equal("200", result);
    }
    
    [Fact]
    public async Task ProcessCommonMessageAsync_RegistersDeviceAndSensorName()
    {
      await SeedDefaultSensor();
      var message = new MqttMessage("iot/device-new/sensor-new", "120", _clock.GetCurrentInstant());

      await _service.ProcessCommonMessageAsync(message);

      Assert.Contains("device-new", await _mqttIdentifierService.ListDeviceNamesAsync());
      Assert.Contains("sensor-new", await _mqttIdentifierService.ListSensorNamesByDeviceNameAsync("device-new"));
    }

    [Fact]
    public async Task ProcessCommonMessageAsync_DoesNotRegisterDeviceNameWithoutProperPrefix()
    {
      var message = new MqttMessage("prefix/device-alpha/sensor-beta", "120", _clock.GetCurrentInstant());

      await _service.ProcessCommonMessageAsync(message);

      Assert.DoesNotContain("device-new", await _mqttIdentifierService.ListDeviceNamesAsync());
    }

    [Fact]
    public async Task ProcessCommonMessageAsync_DoesNotRegisterInProperDeviceName()
    {
      foreach (var deviceName in new string[]{"/", ""})
      {
        var message = new MqttMessage($"iot/{deviceName}sensor-10", "120", _clock.GetCurrentInstant());

        await _service.ProcessCommonMessageAsync(message);

        Assert.DoesNotContain(deviceName, await _mqttIdentifierService.ListDeviceNamesAsync());
        Assert.DoesNotContain("sensor-10", await _mqttIdentifierService.ListDeviceNamesAsync());
      }
    }

    [Fact]
    public async Task ProcessCommonMessageAsync_DoesNotRegisterInProperSensorName()
    {
      foreach (var name in new string[]{"/", ""})
      {
        var message = new MqttMessage($"iot/device-name{name}", "120", _clock.GetCurrentInstant());

        await _service.ProcessCommonMessageAsync(message);

        Assert.Contains("device-name", await _mqttIdentifierService.ListDeviceNamesAsync());
        Assert.DoesNotContain(name, await _mqttIdentifierService.ListSensorNamesByDeviceNameAsync("device-name"));
      }
    }


    [Fact]
    public async Task ProcessCommonMessageAsync_DoesNotRegisterIllformedTopic()
    {

      foreach (var name in new string[]{"random", ""})
      {
        var message = new MqttMessage($"iot/{name}", "120", _clock.GetCurrentInstant());

        await _service.ProcessCommonMessageAsync(message);

        Assert.DoesNotContain(name, await _mqttIdentifierService.ListDeviceNamesAsync());
      }
    }

    [Fact]
    public async Task ProcessCommonMessageAsync_DoesNotStoreWhenNotRegisteredSensor()
    {
      await SeedDefaultSensor();
      var message = new MqttMessage("iot/not-exist-device/not-exist-sensor", "120", _clock.GetCurrentInstant());

      await _service.ProcessCommonMessageAsync(message);

      var cnt = await _dbContext.SensorValues.CountAsync();
      Assert.Equal(0, cnt);
    }

    [Fact]
    public async Task ProcessCommonMessageAsync_StoresValueIfRegistered()
    {
      await SeedDefaultSensor();
      var message = new MqttMessage("iot/device-alpha/sensor-beta", "120.9", _clock.GetCurrentInstant());

      await _service.ProcessCommonMessageAsync(message); 

      var v = await _dbContext.SensorValues.FirstOrDefaultAsync();
      var eps = 1e-7;
      Assert.True(v.Value > 120.9 - eps && v.Value < 120.9 + eps);
    }

    [Fact]
    public async Task ProcessCommonMessageAsync_QueuesUpdatingTimestampInJob()
    {
      throw new NotImplementedException(); 
    }

  }
}
