using System;
using SMEIoT.Core.Interfaces;
using SMEIoT.Core.Services;
using Xunit;
using System.Threading.Tasks;

namespace SMEIoT.Tests.Core.Services
{
  public class SecureKeySuggestionServiceTest
  {
    private readonly SecureKeySuggestionService _service;

    public SecureKeySuggestionServiceTest()
    {
      _service = new SecureKeySuggestionService();
    }

    [Fact]
    public async Task GenerateSecureKeyWithByteLength_ReturnsKey()
    {
      // arrange

      // act
      var key = await _service.GenerateSecureKeyWithByteLengthAsync(SecureKeySuggestionService.ByteLengthUpperBound);

      // assert
      Assert.Equal(SecureKeySuggestionService.ByteLengthUpperBound*2, key.Length);
    }

    [Fact]
    public async Task GenerateSecureKeyWithByteLength_ReturnsKeyAtLowerBound()
    {
      var key = await _service.GenerateSecureKeyWithByteLengthAsync(SecureKeySuggestionService.ByteLengthLowerBound);

      Assert.Equal(SecureKeySuggestionService.ByteLengthLowerBound * 2, key.Length);
    }

    [Fact]
    public async Task GenerateSecureKeyWithByteLength_ThrowsIfLargeThanUpperBound()
    {

      Task Act() => _service.GenerateSecureKeyWithByteLengthAsync(SecureKeySuggestionService.ByteLengthUpperBound+1);

      var exce = await Record.ExceptionAsync(Act);
      Assert.IsType<ArgumentOutOfRangeException>(exce);
    }

    [Fact]
    public async Task GenerateSecureKeyWithByteLength_ThrowsIfLowerThanLowerBound()
    {

      Task Act() => _service.GenerateSecureKeyWithByteLengthAsync(SecureKeySuggestionService.ByteLengthLowerBound-1);

      var exce = await Record.ExceptionAsync(Act);
      Assert.IsType<ArgumentOutOfRangeException>(exce);
    }
  }
}
