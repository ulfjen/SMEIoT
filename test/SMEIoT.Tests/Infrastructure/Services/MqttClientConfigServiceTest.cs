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

namespace SMEIoT.Tests.Core.Services
{
  public class MqttClientConfigServiceTest
  {
    private Mock<IConfigurationSection> _sectionMock;
    private Mock<IConfigurationSection> _emptyMock;

    public MqttClientConfigServiceTest()
    {
      _sectionMock = new Mock<IConfigurationSection>();
      _sectionMock.Setup(s => s["MqttHost"]).Returns("127.0.0.1");
      _sectionMock.Setup(s => s["MqttPort"]).Returns("1235");
      _emptyMock = new Mock<IConfigurationSection>();
      _emptyMock.Setup(s => s[It.IsAny<string>()]).Returns("");
    }

    [Fact]
    public void Host_Returns()
    {
      // arrange
      var mock = new Mock<IConfiguration>();
      mock.Setup(x => x.GetSection("ConnectionStrings")).Returns(_sectionMock.Object);
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
      mock.Setup(x => x.GetSection("ConnectionStrings")).Returns(_emptyMock.Object);
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
      mock.Setup(x => x.GetSection("ConnectionStrings")).Returns(_sectionMock.Object);
      var service = new MqttClientConfigService(mock.Object);

      var res = service.GetPort();

      Assert.Equal(1235, res);
    }

    [Fact]
    public void Port_ThrowsIfValueNotSet()
    {
      var mock = new Mock<IConfiguration>();
      mock.Setup(x => x.GetSection("ConnectionStrings")).Returns(_emptyMock.Object);
      var service = new MqttClientConfigService(mock.Object);

      Action act = () => service.GetPort();

      // assert
      var exce = Assert.Throws<InvalidOperationException>(act);
      Assert.Contains("MqttPort", exce.Message);
    }
  }
}
