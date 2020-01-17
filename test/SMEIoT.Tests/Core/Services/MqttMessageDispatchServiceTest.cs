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
  public class MqttMessageDispatchServiceTest
  {
    private readonly Instant _initial;
    private readonly IClock _clock;
    private readonly Mock<IServiceScopeFactory> _scopeFactoryMock;
    private readonly Mock<IServiceScope> _scopeMock;
    private readonly MqttMessageDispatchService _service;
    private readonly Mock<IMqttMessageIngestionService> _ingestMock;
    private readonly Mock<IMqttMessageRelayService> _relayMock;

    public MqttMessageDispatchServiceTest()
    {
      _scopeFactoryMock = new Mock<IServiceScopeFactory>();
      _scopeMock = new Mock<IServiceScope>();
      _ingestMock = new Mock<IMqttMessageIngestionService>();
      _relayMock = new Mock<IMqttMessageRelayService>();

      var serviceCollection = new ServiceCollection();
      serviceCollection.AddScoped<IMqttMessageIngestionService>(provider => _ingestMock.Object);
      serviceCollection.AddScoped<IMqttMessageRelayService>(provider => _relayMock.Object);
      _scopeMock.Setup(s => s.ServiceProvider).Returns(serviceCollection.BuildServiceProvider());
      _scopeFactoryMock.Setup(s => s.CreateScope()).Returns(_scopeMock.Object);
      _service = new MqttMessageDispatchService(_scopeFactoryMock.Object);

      _initial = SystemClock.Instance.GetCurrentInstant();
      _clock = new FakeClock(_initial);
    }

    [Fact]
    public async Task ProcessAsync_CreatesNewScopeAndUsesNewScopedServiceProvider()
    {
      var message = new MqttMessage("iot/device-alpha/sensor-beta", "120", _clock.GetCurrentInstant());

      await _service.ProcessAsync(message);

      _scopeFactoryMock.Verify(s => s.CreateScope(), Times.Once());
      _scopeMock.Verify(s => s.ServiceProvider, Times.Exactly(2));
    }

    [Fact]
    public async Task ProcessAsync_CallsOtherService()
    {
      var message = new MqttMessage("iot/device-alpha/sensor-beta", "120", _clock.GetCurrentInstant());

      await _service.ProcessAsync(message);

      _ingestMock.Verify(s => s.ProcessCommonMessageAsync(message), Times.Once());
      _ingestMock.Verify(s => s.ProcessBrokerMessageAsync(message), Times.Once());
      _relayMock.Verify(s => s.RelayAsync(message), Times.Once());
    }
  }
}
