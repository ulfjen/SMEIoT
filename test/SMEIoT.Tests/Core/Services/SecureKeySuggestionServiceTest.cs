using System;
using SMEIoT.Core.Interfaces;
using SMEIoT.Core.Services;
using Xunit;
using System.Threading.Tasks;

namespace SMEIoT.Tests.Core.Services
{
  public class SecureKeySuggestionServiceTest
  {
    private static SecureKeySuggestionService BuildService()
    {
      return new SecureKeySuggestionService();
    }

    [Fact]
    public async Task GenerateSecureKeyWithByteLength_ReturnsKey()
    {
      // arrange
      var service = BuildService();

      // act
      var key = await service.GenerateSecureKeyWithByteLengthAsync(SecureKeySuggestionService.ByteLengthUpperBound);

      // assert
      Assert.Equal(SecureKeySuggestionService.ByteLengthUpperBound*2, key.Length);
    }

    [Fact]
    public async Task GenerateSecureKeyWithByteLength_ThrowsIfLargeThanUpperBound()
    {
      var service = BuildService();

      Task Act() => service.GenerateSecureKeyWithByteLengthAsync(SecureKeySuggestionService.ByteLengthUpperBound-1);

      var exce = await Record.ExceptionAsync(Act);
      Assert.IsType<ArgumentOutOfRangeException>(exce);
    }

    [Fact]
    public async Task GenerateSecureKeyWithByteLength_ThrowsIfLowerThanLowerBound()
    {
      var service = BuildService();

      Task Act() => service.GenerateSecureKeyWithByteLengthAsync(SecureKeySuggestionService.ByteLengthLowerBound-1);

      var exce = await Record.ExceptionAsync(Act);
      Assert.IsType<ArgumentOutOfRangeException>(exce);
    }
  }
}
