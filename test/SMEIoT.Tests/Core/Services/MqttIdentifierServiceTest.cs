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
  public class MqttIdentifierServiceTest
  {
    private static async Task<MqttIdentifierService> BuildService()
    {
      var dbContext = ApplicationDbContextHelper.BuildTestDbContext();

      return new MqttIdentifierService(new FakeClock(Instant.FromUtc(2020, 1, 1, 0, 0)));
    }

    [Fact]
    public async Task RegisterDeviceNameAsync_ReturnsSuccess()
    {
      // arrange
      var service = await BuildService();

      // act
      var res = service.RegisterDeviceName("L401");

      // assert
      Assert.True(res);
    }

    [Fact]
    public async Task RegisterDeviceNameAsync_ReturnsFalseWhenEmpty()
    {
      var service = await BuildService();

      var res = service.RegisterDeviceName("");

      Assert.False(res);
    }


    [Fact]
    public async Task ListDeviceNames_ListNothing()
    {
      var service = await BuildService();

      var list = service.ListDeviceNames();

      Assert.Empty(list);
    }

    [Fact]
    public async Task ListDeviceNames_ListAddedDevice()
    {
      var service = await BuildService();
      service.RegisterDeviceName("L401");
      service.RegisterDeviceName("L427");

      var list = service.ListDeviceNames();

      Assert.Contains("L401", list);
      Assert.Contains("L427", list);
    }
  }
}
