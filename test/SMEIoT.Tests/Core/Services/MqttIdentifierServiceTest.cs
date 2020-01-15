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
using Xunit;

namespace SMEIoT.Tests.Core.Services
{
  [Collection("Database collection")]
  public class MqttIdentifierServiceTest
  {
    private readonly MqttIdentifierService _service;

    public MqttIdentifierServiceTest()
    {
      _service = new MqttIdentifierService(new FakeClock(Instant.FromUtc(2020, 1, 1, 0, 0)));
    }

    private async Task SeedDefaultDeviceNames()
    {
      await _service.RegisterDeviceNameAsync("L401");
      await _service.RegisterDeviceNameAsync("L427");
    }

    [Fact]
    public async Task RegisterDeviceNameAsync_Returns()
    {
      // arrange

      // act
      await _service.RegisterDeviceNameAsync("L401");

      // assert
      // no throws
    }

    [Fact]
    public async Task RegisterDeviceNameAsync_ThrowsWhenEmpty()
    {

      Task Act() => _service.RegisterDeviceNameAsync("");

      var exce = await Record.ExceptionAsync(Act);
      var details = Assert.IsType<InvalidArgumentException>(exce);
      Assert.Equal("name", details.ParamName);
    }

    [Fact]
    public async Task ListDeviceNamesAsync_ListNothing()
    {

      var list = await _service.ListDeviceNamesAsync();

      Assert.Empty(list);
    }

    [Fact]
    public async Task RegisterSensorNameWithDeviceNameAsync_Returns()
    {
      await SeedDefaultDeviceNames();

      await _service.RegisterSensorNameWithDeviceNameAsync("temp", "L401");

    }

    [Fact]
    public async Task RegisterSensorNameWithDeviceNameAsync_ThrowsIfEmptySensorName()
    {
      await SeedDefaultDeviceNames();

      Task Act() => _service.RegisterSensorNameWithDeviceNameAsync("", "L401");

      var exce = await Record.ExceptionAsync(Act);
      var details = Assert.IsType<InvalidArgumentException>(exce);
      Assert.Equal("name", details.ParamName);
    }

    [Fact]
    public async Task RegisterSensorNameWithDeviceNameAsync_ThrowsIfNotRegisteredDeviceName()
    {
      await SeedDefaultDeviceNames();

      Task Act() => _service.RegisterSensorNameWithDeviceNameAsync("temp", "L");

      var exce = await Record.ExceptionAsync(Act);
      var details = Assert.IsType<InvalidArgumentException>(exce);
      Assert.Equal("deviceName", details.ParamName);
    }

    [Fact]
    public async Task ListSensorNamesByDeviceNameAsync_ListNothing()
    {
      await SeedDefaultDeviceNames();

      var list = await _service.ListSensorNamesByDeviceNameAsync("L401");

      Assert.Empty(list);
    }

    [Fact]
    public async Task ListSensorNamesByDeviceNameAsync_ListNothingWhenWrongIdentifierProvided()
    {
      await SeedDefaultDeviceNames();

      var list = await _service.ListSensorNamesByDeviceNameAsync("");

      Assert.Empty(list);
    }

    [Fact]
    public async Task ListSensorNamesByDeviceNameAsync_Lists()
    {
      await SeedDefaultDeviceNames();
      await _service.RegisterSensorNameWithDeviceNameAsync("temp", "L401");

      var list = await _service.ListSensorNamesByDeviceNameAsync("L401");

      Assert.Single(list);
      Assert.Equal("temp", list.First());
    }

  }
}
