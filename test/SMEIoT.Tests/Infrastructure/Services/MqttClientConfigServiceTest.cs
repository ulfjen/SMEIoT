using System;
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

namespace SMEIoT.Tests.Core
{
  public class MqttClientConfigServiceTest
  {
    private Mock<IConfigurationSection> _sectionMock;
    private Mock<IConfigurationSection> _emptyMock;

    public MqttClientConfigServiceTest()
    {
      _sectionMock = new Mock<IConfigurationSection>();
      var hostMock = new Mock<IConfigurationSection>();
      hostMock.Setup(s => s.Value).Returns("127.0.0.1");
      var portMock = new Mock<IConfigurationSection>();
      portMock.Setup(s => s.Value).Returns("1235");
      _sectionMock.Setup(s => s.GetSection("MqttHost")).Returns(hostMock.Object);
      _sectionMock.Setup(s => s.GetSection("MqttPort")).Returns(portMock.Object);
      _emptyMock = new Mock<IConfigurationSection>();
      var emptyVal = new Mock<IConfigurationSection>();
      emptyVal.Setup(s => s.Value).Returns("");
      _emptyMock.Setup(s => s.GetSection(It.IsAny<string>())).Returns(emptyVal.Object);
    }

    [Fact]
    public void Host_Returns()
    {
      // arrange
      var mock = new Mock<IConfiguration>();
      mock.Setup(x => x.GetSection("SMEIoT")).Returns(_sectionMock.Object);
      var service = new MqttClientConfigService(mock.Object);

      // act
      var res = service.GetHost();

      // assert
      Assert.Equal("127.0.0.1", res);
    }

    [Fact]
    public void Host_ThrowsIfValueNotSet()
    {
      var mock = new Mock<IConfiguration>();
      mock.Setup(x => x.GetSection("SMEIoT")).Returns(_emptyMock.Object);
      var service = new MqttClientConfigService(mock.Object);

      Action act = () => service.GetHost();

      // assert
      var exce = Assert.Throws<InvalidOperationException>(act);
      Assert.Contains("MqttHost", exce.Message);
    }

    [Fact]
    public void Port_Returns()
    {
      var mock = new Mock<IConfiguration>();
      mock.Setup(x => x.GetSection("SMEIoT")).Returns(_sectionMock.Object);
      var service = new MqttClientConfigService(mock.Object);

      var res = service.GetPort();

      Assert.Equal(1235, res);
    }

    [Fact]
    public void Port_ThrowsIfValueNotSet()
    {
      var mock = new Mock<IConfiguration>();
      mock.Setup(x => x.GetSection("SMEIoT")).Returns(_emptyMock.Object);
      var service = new MqttClientConfigService(mock.Object);

      Action act = () => service.GetPort();

      // assert
      var exce = Assert.Throws<InvalidOperationException>(act);
      Assert.Contains("MqttPort", exce.Message);
    }

    [Fact]
    public async Task SuggestConfigAsync_Suggests()
    {
      var mock = new Mock<IConfiguration>();
      mock.Setup(x => x.GetSection("SMEIoT")).Returns(_sectionMock.Object);
      var service = new MqttClientConfigService(mock.Object);

      var config = await service.SuggestConfigAsync();

      Assert.Equal("127.0.0.1", config.Host);
      Assert.Equal(1235, config.Port);
      Assert.Equal("iot/", config.TopicPrefix);
    } 
  }
}
