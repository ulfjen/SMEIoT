using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SMEIoT.Core.Services;
using Xunit;
using NodaTime;
using NodaTime.Testing;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Moq;
using SMEIoT.Core.Interfaces;
using SMEIoT.Web.Services;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.HostFiltering;

namespace SMEIoT.Tests.Web.Services
{
  public class ServerHostAccessorTest
  {
    private Mock<IServerNetworkInterfacesIpAccessor> _accessorMock;
    private Mock<IOptions<HostFilteringOptions>> _optionMock;
    private readonly ServerHostAccessor _service;

    public ServerHostAccessorTest()
    {
      _optionMock = new Mock<IOptions<HostFilteringOptions>>();
      _optionMock.SetupGet(o => o.Value).Returns(new HostFilteringOptions {
        AllowedHosts = new List<string> { "smeiot.se" }
      });
      _accessorMock = new Mock<IServerNetworkInterfacesIpAccessor>();
      _accessorMock.Setup(a => a.GetNetworkInterfacesIpString()).Returns(new []{"193.100.10.1"});
      _service = new ServerHostAccessor(_accessorMock.Object, _optionMock.Object);
    }

    [Fact]
    public async Task GetServerHostAsync_GetServerHost()
    {

      var host = await _service.GetServerHostAsync();

      Assert.Equal("smeiot.se", host);
    }

    [Fact]
    public async Task GetServerHostAsync_GetFirstServerHostIfMultiple()
    {
      var optionMock = new Mock<IOptions<HostFilteringOptions>>();
      optionMock.SetupGet(o => o.Value).Returns(new HostFilteringOptions {
        AllowedHosts = new List<string> { "example.com", "smeiot.se" }
      });
      var service = new ServerHostAccessor(_accessorMock.Object, optionMock.Object);

      var host = await service.GetServerHostAsync();

      Assert.Equal("example.com", host);
    }

    [Fact]
    public async Task GetServerHostAsync_GetIpIfServerHostAllowsEverything()
    {
      var optionMock = new Mock<IOptions<HostFilteringOptions>>();
      optionMock.SetupGet(o => o.Value).Returns(new HostFilteringOptions {
        AllowedHosts = new List<string> { "*" }
      });
      var service = new ServerHostAccessor(_accessorMock.Object, optionMock.Object);

      var host = await service.GetServerHostAsync();

      Assert.Equal("193.100.10.1", host);
    }


    [Fact]
    public async Task GetServerHostAsync_GetIpIfNoAllowedHosts()
    {
      var optionMock = new Mock<IOptions<HostFilteringOptions>>();
      optionMock.SetupGet(o => o.Value).Returns(new HostFilteringOptions {
        AllowedHosts = new List<string> { }
      });
      var service = new ServerHostAccessor(_accessorMock.Object, optionMock.Object);

      var host = await service.GetServerHostAsync();

      Assert.Equal("193.100.10.1", host);
    }

    [Fact]
    public async Task GetServerHostAsync_ReturnsNullIfDectionFails()
    {
      var optionMock = new Mock<IOptions<HostFilteringOptions>>();
      optionMock.SetupGet(o => o.Value).Returns(new HostFilteringOptions {
        AllowedHosts = null
      });
      var accessorMock = new Mock<IServerNetworkInterfacesIpAccessor>();
      accessorMock.Setup(a => a.GetNetworkInterfacesIpString()).Returns(new string[]{});
      var service = new ServerHostAccessor(accessorMock.Object, optionMock.Object);

      var host = await service.GetServerHostAsync();

      Assert.Null(host);
    }

  }
}
