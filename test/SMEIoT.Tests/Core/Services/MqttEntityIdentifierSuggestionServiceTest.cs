using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NodaTime;
using NodaTime.Testing;
using SMEIoT.Core.Interfaces;
using SMEIoT.Core.Services;
using Xunit;

namespace SMEIoT.Tests.Core.Services
{
  public class MqttEntityIdentifierSuggestionServiceTest
  {
    private static MqttEntityIdentifierSuggestionService BuildService()
    {
      var mockedAccessor = new Mock<IIdentifierDictionaryFileAccessor>();
      mockedAccessor.Setup(x => x.ListIdentifiers()).Returns(new List<string> { "id1", "id2", "id3" });
      var identifierService = new MqttIdentifierService(new FakeClock(Instant.FromUtc(2020, 1, 1, 0, 0)));
      identifierService.RegisterSensorNameWithDeviceName("sensor1", "device1");
      identifierService.RegisterSensorNameWithDeviceName("sensor2", "device1");
      identifierService.RegisterSensorNameWithDeviceName("sensor3", "device2");

      var mockedDb = new Mock<IApplicationDbConnection>(MockBehavior.Strict);
      mockedDb.Setup(x => x.ExecuteScalar<bool>("SELECT COUNT(DISTINCT 1) FROM devices WHERE normalized_name = @NormalizedName;", It.IsAny<object>(), null, null, null)).Returns(false);
      return new MqttEntityIdentifierSuggestionService(identifierService, mockedAccessor.Object, mockedDb.Object);
    }

    [Fact]
    public async Task GenerateRandomIdentifierForDevice_ReturnsIdentifier()
    {
      // arrange
      var service = BuildService();

      // act
      var res = await service.GenerateRandomIdentifierForDeviceAsync(1);

      // assert
      Assert.True(res == "id1" || res == "id2" || res == "id3");
    }

    [Fact]
    public async Task GenerateRandomIdentifierForDevice_ThrowsIfLessOneWord()
    {
      var service = BuildService();

      Task Act() => service.GenerateRandomIdentifierForDeviceAsync(0);

      var exce = await Record.ExceptionAsync(Act);
      Assert.IsType<ArgumentException>(exce);
    }


    [Fact]
    public async Task GetARandomIdentifierCandidatesForSensor_ReturnsCandidate()
    {
      var service = BuildService();

      var cand = service.GetARandomIdentifierCandidatesForSensor("device2");

      Assert.Equal("sensor3", cand);
    }


    [Fact]
    public void GetARandomIdentifierCandidatesForSensor_ReturnsOneCandidate()
    {
      var service = BuildService();

      var cand = service.GetARandomIdentifierCandidatesForSensor("device1");

      Assert.True(cand == "sensor1" || cand == "sensor2");
    }

    [Fact]
    public void GetARandomIdentifierCandidatesForSensor_ReturnsNull()
    {
      var service = BuildService();

      var cand = service.GetARandomIdentifierCandidatesForSensor("device-not-existed");

      Assert.Null(cand);
    }


  }
}
