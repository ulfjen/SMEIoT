using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NodaTime;
using NodaTime.Testing;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Exceptions;
using SMEIoT.Core.Interfaces;
using SMEIoT.Core.Services;
using SMEIoT.Infrastructure.Data;
using SMEIoT.Tests.Shared;
using Xunit;

namespace SMEIoT.Tests.Core.Services
{
  public class MqttEntityIdentifierSuggestionServiceTest
  {
    private readonly ApplicationDbContext _dbContext;
    private readonly MqttEntityIdentifierSuggestionService _service;

    public MqttEntityIdentifierSuggestionServiceTest()
    {
      var mockedAccessor = new Mock<IIdentifierDictionaryFileAccessor>();
      mockedAccessor.Setup(x => x.ListIdentifiers(It.IsAny<string>())).Returns(new List<string> { "id1", "id2", "id3" });
      var identifierService = new MqttIdentifierService(new FakeClock(Instant.FromUtc(2020, 1, 1, 0, 0)));
      identifierService.RegisterSensorNameWithDeviceName("sensor1", "device1");
      identifierService.RegisterSensorNameWithDeviceName("sensor2", "device1");
      identifierService.RegisterSensorNameWithDeviceName("sensor3", "device2");

      _dbContext = ApplicationDbContextHelper.BuildTestDbContext();
      _service = new MqttEntityIdentifierSuggestionService(identifierService, mockedAccessor.Object, _dbContext);
    }

    [Fact]
    public async Task GenerateRandomIdentifierForDevice_ReturnsIdentifier()
    {
      // arrange

      // act
      var res = await _service.GenerateRandomIdentifierForDeviceAsync(1);

      // assert
      Assert.True(res == "id1" || res == "id2" || res == "id3");
    }

    [Fact]
    public async Task GenerateRandomIdentifierForDevice_DontReturnDeviceName()
    {
      var mockedAccessor = new Mock<IIdentifierDictionaryFileAccessor>();
      mockedAccessor.Setup(x => x.ListIdentifiers(It.IsAny<string>())).Returns(new List<string> { "id1", "id2" });
      var identifierService = new MqttIdentifierService(new FakeClock(Instant.FromUtc(2020, 1, 1, 0, 0)));
      identifierService.RegisterSensorNameWithDeviceName("sensor1", "device1");
      identifierService.RegisterSensorNameWithDeviceName("sensor2", "device1");
      identifierService.RegisterSensorNameWithDeviceName("sensor3", "device2");

      var service = new MqttEntityIdentifierSuggestionService(identifierService, mockedAccessor.Object, _dbContext);
      _dbContext.Devices.Add(new Device { Name = "id1", NormalizedName = Device.NormalizeName("id1") });
      await _dbContext.SaveChangesAsync();

      var res = await service.GenerateRandomIdentifierForDeviceAsync(1);

      Assert.True(res != "id1");
      Assert.True(res == "id2");
    }

    [Fact]
    public async Task GenerateRandomIdentifierForDevice_ThrowsIfLessOneWord()
    {

      Task Act() => _service.GenerateRandomIdentifierForDeviceAsync(0);

      var exce = await Record.ExceptionAsync(Act);
      var details = Assert.IsType<InvalidArgumentException>(exce);
      Assert.Equal("numWords", details.ParamName);
    }

    [Fact]
    public async Task GetOneIdentifierCandidateForSensorAsync_ReturnsCandidate()
    {

      var cand = await _service.GetOneIdentifierCandidateForSensorAsync("device2");

      Assert.Equal("sensor3", cand);
    }


    [Fact]
    public async Task GetOneIdentifierCandidateForSensorAsync_ReturnsOneCandidate()
    {

      var cand = await _service.GetOneIdentifierCandidateForSensorAsync("device1");

      Assert.True(cand == "sensor1" || cand == "sensor2");
    }

    [Fact]
    public async Task GetOneIdentifierCandidateForSensorAsync_ReturnsNull()
    {

      var cand = await _service.GetOneIdentifierCandidateForSensorAsync("device-not-existed");

      Assert.Null(cand);
    }


  }
}
