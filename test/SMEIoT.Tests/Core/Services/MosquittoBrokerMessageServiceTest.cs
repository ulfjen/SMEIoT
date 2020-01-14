using System;
using System.Text;
using System.Threading.Tasks;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Exceptions;
using SMEIoT.Core.Services;
using SMEIoT.Infrastructure.Data;
using SMEIoT.Tests.Shared;
using Xunit;
using NodaTime;
using NodaTime.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace SMEIoT.Tests.Core.Services
{
  public class MosquittoBrokerMessageServiceTest : IDisposable
  {
    private readonly ApplicationDbContext _dbContext;
    private Instant _initial;
    private readonly MosquittoBrokerMessageService _service;
    private readonly DeviceService _deviceService;
    private readonly MosquittoClientAuthenticationService _clientAuthService;
    
    public MosquittoBrokerMessageServiceTest()
    {
      _initial = SystemClock.Instance.GetCurrentInstant();
      _dbContext = ApplicationDbContextHelper.BuildTestDbContext(_initial);
      var keyService = new SecureKeySuggestionService();
      _clientAuthService = new MosquittoClientAuthenticationService(keyService);
      var clock = new FakeClock(_initial);
      var brokerService = new MosquittoBrokerService(clock, new NullLogger<MosquittoBrokerService>()); 
      _deviceService = new DeviceService(_dbContext);
      _service = new MosquittoBrokerMessageService(_clientAuthService, brokerService, _deviceService);
    }

    public void Dispose()
    {
      _dbContext.Database.ExecuteSqlInterpolated($"TRUNCATE devices CASCADE;");
      _dbContext.Dispose();
    }

    [Fact]
    public async Task ProcessDecodedMessageAsync_RespondsToMeta()
    {
      // arrange
      
      // act
      var builder = await _service.ProcessDecodedMessageAsync("POSTMETA {\"mosquittoAuthPluginVersion\":4,\"pid\":2353}");
      
      // assert
      Assert.Equal("OK", builder.ToString()); 
    }
   
    [Fact]
    public async Task ProcessDecodedMessageAsync_RespondsToNotExistDevice()
    {
      var builder = await _service.ProcessDecodedMessageAsync("GETPSK not-exists-device");

      Assert.Equal("FAIL", builder.ToString()); 
    }

    [Fact]
    public async Task ProcessDecodedMessageAsync_RespondsToExistDevice()
    {
      await _deviceService.BootstrapDeviceWithPreSharedKeyAsync("normal-device", "psk1");

      var builder = await _service.ProcessDecodedMessageAsync("GETPSK normal-device");

      Assert.Equal("psk1", builder.ToString()); 
    }

    [Fact]
    public async Task ProcessDecodedMessageAsync_RespondsToSMEIoTClient()
    {
      var name = await _clientAuthService.GetClientNameAsync();
      var psk = await _clientAuthService.GetClientPskAsync();

      var builder = await _service.ProcessDecodedMessageAsync($"GETPSK {name}");

      Assert.Equal(psk, builder.ToString()); 
    }

    [Fact]
    public async Task ProcessDecodedMessageAsync_RespondsToUnknownCommand()
    {
      var builder = await _service.ProcessDecodedMessageAsync("PUT normal-device");

      Assert.Equal("FAIL", builder.ToString());
    }
  }
}
