using System;
using System.Threading.Tasks;
using SMEIoT.Core.Services;
using Xunit;
using NodaTime;
using NodaTime.Testing;
using Microsoft.Extensions.Logging.Abstractions;
using System.Linq;
using Moq;
using SMEIoT.Core.Interfaces;

namespace SMEIoT.Tests.Core.Services
{
  public class MosquittoBrokerServiceTest
  {
    private readonly Instant _initial;
    private readonly MosquittoBrokerService _service;
    
    public MosquittoBrokerServiceTest()
    {
      _initial = SystemClock.Instance.GetCurrentInstant();
      var mock = new Mock<IMosquittoBrokerPidAccessor>();
      mock.SetupGet(a => a.BrokerPid).Returns(10000);
      var mockPlugin = new Mock<IMosquittoBrokerPluginPidService>();
      mockPlugin.SetupGet(a => a.BrokerPidFromAuthPlugin).Returns(10000);

      _service = new MosquittoBrokerService(new FakeClock(_initial), new NullLogger<MosquittoBrokerService>(), mock.Object, mockPlugin.Object);
    }

    [Fact]
    public async Task RegisterBrokerStatistics_RegistersValue()
    {
      // arrange

      // act
      var res = await _service.RegisterBrokerStatisticsAsync("name", "value", _initial);

      // assert
      Assert.True(res);
      Assert.Equal("value", await _service.GetBrokerStatisticsAsync("name"));
    }

    [Fact]
    public async Task RegisterBrokerStatistics_RegistersValueWithTime()
    {

      var res = await _service.RegisterBrokerStatisticsAsync("name", "value", _initial);

      Assert.True(res);
      Assert.Equal(_initial, _service.BrokerLastMessageAt);
    }

    [Fact]
    public async Task RegisterBrokerStatistics_OverridesTime()
    {
      var newTime = _initial + Duration.FromDays(1);
      await _service.RegisterBrokerStatisticsAsync("name", "value", _initial);

      var res = await _service.RegisterBrokerStatisticsAsync("name", "value", newTime);

      // assert
      Assert.True(res);
      Assert.Equal(newTime, _service.BrokerLastMessageAt);
    }

    [Fact]
    public async Task RegisterBrokerStatistics_UpdatesLastMessageAt()
    {
      Assert.Null(_service.BrokerLastMessageAt);
      
      var res = await _service.RegisterBrokerStatisticsAsync("name", "value", _initial);

      Assert.True(res);
      Assert.Equal(_initial, _service.BrokerLastMessageAt);
    }

    [Fact]
    public async Task GetBrokerStatisticsAsync_ReturnsNullWhenNoKeyRegisters()
    {

      var res = await _service.GetBrokerStatisticsAsync("non-exists");

      Assert.Null(res);
    }

    [Fact]
    public async Task GetBrokerStatisticsAsync_ReturnsValue()
    {
      await _service.RegisterBrokerStatisticsAsync("name", "value", _initial);

      var res = await _service.GetBrokerStatisticsAsync("name");

      Assert.Equal("value", res);
    }

    [Fact]
    public async Task ListBrokerStatisticsAsync_ReturnsValues()
    {
      await _service.RegisterBrokerStatisticsAsync("name", "value", _initial);
      await _service.RegisterBrokerStatisticsAsync("name1", "value", _initial);

      var res = await _service.ListBrokerStatisticsAsync();

      var l = res.ToList();
      Assert.Equal(2, l.Count);
      Assert.Contains(l, pair => pair.Key == "name");
      Assert.Contains(l, pair => pair.Key == "name1");
    }

    [Fact]
    public async Task GetBrokerLoadAsync_ReturnsNullWhenNoRelatedStatisticsIsReady()
    {
      await _service.RegisterBrokerStatisticsAsync("name", "value", _initial);

      var (min1, min5, min15) = await _service.GetBrokerLoadAsync();

      Assert.Null(min1);
      Assert.Null(min5);
      Assert.Null(min15);
    }

    [Fact]
    public async Task GetBrokerLoadAsync_ReturnsLoad()
    {
      await _service.RegisterBrokerStatisticsAsync("load/bytes/received/1min", "100", _initial);
      await _service.RegisterBrokerStatisticsAsync("load/bytes/sent/1min", "200", _initial);
      await _service.RegisterBrokerStatisticsAsync("load/bytes/received/5min", "400", _initial);
      await _service.RegisterBrokerStatisticsAsync("load/bytes/sent/5min", "600", _initial);
      await _service.RegisterBrokerStatisticsAsync("load/bytes/received/15min", "800", _initial);
      await _service.RegisterBrokerStatisticsAsync("load/bytes/sent/15min", "1000", _initial);

      var (min1, min5, min15) = await _service.GetBrokerLoadAsync();

      var eps = 1e-7;
      Assert.True(150.0 - eps < min1 && min1 < 150.0 + eps);
      Assert.True(500.0 - eps < min5 && min5 < 500.0 + eps);
      Assert.True(900.0 - eps < min15 && min15 < 900.0 + eps);
    }

    [Fact]
    public async Task GetBrokerLoadAsync_ReturnsPartialLoad()
    {
      await _service.RegisterBrokerStatisticsAsync("load/bytes/received/1min", "100", _initial);
      await _service.RegisterBrokerStatisticsAsync("load/bytes/sent/1min", "200", _initial);

      var (min1, min5, min15) = await _service.GetBrokerLoadAsync();

      var eps = 1e-7;
      Assert.True(150.0 - eps < min1 && min1 < 150.0 + eps);
      Assert.Null(min5);
      Assert.Null(min15);
    }

    [Fact]
    public async Task GetBrokerLoadAsync_ReturnsPartialLoadWhenSomeValueIsMissing()
    {
      await _service.RegisterBrokerStatisticsAsync("load/bytes/received/1min", "100", _initial);

      var (min1, min5, min15) = await _service.GetBrokerLoadAsync();

      Assert.Null(min1);
      Assert.Null(min5);
      Assert.Null(min15);
    }

    [Fact]
    public async Task BrokerRunning_IsRunningIfReceivingMessage()
    {
      Assert.False(_service.BrokerRunning);

      await _service.RegisterBrokerStatisticsAsync("load/bytes/received/1min", "100", _initial);

      Assert.True(_service.BrokerRunning);
    }

    [Fact]
    public async Task BrokerRunning_IsRunningIfReceivedMessageInAWindow()
    {

      await _service.RegisterBrokerStatisticsAsync("load/bytes/received/1min", "100", _initial-Duration.FromSeconds(1));

      Assert.True(_service.BrokerRunning);
    }

    [Fact]
    public async Task BrokerRunning_IsNotRunningIfReceivedMessageInTheAncientTime()
    {

      await _service.RegisterBrokerStatisticsAsync("load/bytes/received/1min", "100", _initial - Duration.FromDays(1000));

      Assert.False(_service.BrokerRunning);
    }

    private async Task<MosquittoBrokerService> SetupServiceWithPidAsync(Mock<IMosquittoBrokerPidAccessor> mock, Mock<IMosquittoBrokerPluginPidService> mockPlugin)
    {
      var service = new MosquittoBrokerService(new FakeClock(_initial), new NullLogger<MosquittoBrokerService>(), mock.Object, mockPlugin.Object);

      await service.RegisterBrokerStatisticsAsync("load/bytes/received/1min", "100", _initial);
      return service;
    }

    [Fact]
    public async Task BrokerRunning_IsNotRunningWhenPidMismatch()
    {
      var mock = new Mock<IMosquittoBrokerPidAccessor>();
      mock.SetupGet(a => a.BrokerPid).Returns(1000);
      var mockPlugin = new Mock<IMosquittoBrokerPluginPidService>();
      mockPlugin.SetupGet(a => a.BrokerPidFromAuthPlugin).Returns(10000);

      var service = await SetupServiceWithPidAsync(mock, mockPlugin);

      Assert.False(_service.BrokerRunning);
    }

    [Fact]
    public async Task BrokerRunning_IsNotRunningWhenPidNotSet()
    {
      var mock = new Mock<IMosquittoBrokerPidAccessor>();
      mock.SetupGet(a => a.BrokerPid).Returns(() => null);
      var mockPlugin = new Mock<IMosquittoBrokerPluginPidService>();
      mockPlugin.SetupGet(a => a.BrokerPidFromAuthPlugin).Returns(10000);

      var service = await SetupServiceWithPidAsync(mock, mockPlugin);

      Assert.False(_service.BrokerRunning);
    }

    [Fact]
    public async Task BrokerRunning_IsNotRunningWhenPluginPidNotSet()
    {
      var mock = new Mock<IMosquittoBrokerPidAccessor>();
      mock.SetupGet(a => a.BrokerPid).Returns(10000);

      var mockPlugin = new Mock<IMosquittoBrokerPluginPidService>();
      mockPlugin.SetupGet(a => a.BrokerPidFromAuthPlugin).Returns(() => null);

      var service = await SetupServiceWithPidAsync(mock, mockPlugin);

      Assert.False(_service.BrokerRunning);
    }

  }
}
