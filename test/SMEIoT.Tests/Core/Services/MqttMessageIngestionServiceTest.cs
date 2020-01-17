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
using Xunit;

namespace SMEIoT.Tests.Core.Services
{
  [Collection("Database collection")]
  public class MqttMessageIngestionServiceTest
  {
    private readonly Instant _initial;
    private readonly IClock _clock;
    private readonly ApplicationDbContext _dbContext;
    private readonly Mock<IServiceScopeFactory> _scopeFactoryMock;
    private readonly Mock<IServiceScope> _scopeMock;
    private readonly MqttMessageIngestionService _service;

    public MqttMessageIngestionServiceTest()
    {
      _dbContext = ApplicationDbContextHelper.BuildTestDbContext();

      _scopeFactoryMock = new Mock<IServiceScopeFactory>(MockBehavior.Strict);
      _scopeMock = new Mock<IServiceScope>(MockBehavior.Strict);

      var serviceCollection = new ServiceCollection();
      serviceCollection.AddScoped<IApplicationDbContext>(provider => _dbContext);
      _scopeMock.Setup(s => s.ServiceProvider).Returns(serviceCollection.BuildServiceProvider());
      _scopeFactoryMock.Setup(s => s.CreateScope()).Returns(_scopeMock.Object);
      _service = new MqttMessageIngestionService(_scopeFactoryMock.Object);

      _initial = SystemClock.Instance.GetCurrentInstant();
      _clock = new FakeClock(_initial);
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
    public async Task ProcessAsync_CreatesNewScopeAndUsesNewScopedServiceProvider()
    {
      await SeedDefaultSensor();
      var message = new MqttMessage("iot/device-alpha/sensor-beta", "120", _clock.GetCurrentInstant());

      await _service.ProcessAsync(message);

      _scopeFactoryMock.Verify(s => s.CreateScope(), Times.Once());
      _scopeMock.Verify(s => s.ServiceProvider, Times.Once());
    }

    [Fact]
    public async Task ProcessAsync_DoNothingWhenNotRegisteredSensor()
    {
      await SeedDefaultSensor();
      var message = new MqttMessage("iot/not-exist-device/not-exist-sensor", "120", _clock.GetCurrentInstant());
      var service = new MqttMessageIngestionService(_scopeFactoryMock.Object);

      await _service.ProcessAsync(message);

    }

    [Fact]
    public async Task ProcessAsync_QueueJobForUpdateLastMessageTimestamp()
    {
      
    }

    [Fact]
    public async Task ProcessAsync_StoreValueIfRegisteredSensor()
    {
      
    }

    [Fact]
    public async Task ProcessAsync_ProcessesBrokerMessages()
    {
      
    }
  }
}
