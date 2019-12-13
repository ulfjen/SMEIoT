using System;
using SMEIoT.Core.Interfaces;
using SMEIoT.Core.Services;
using Xunit;

namespace SMEIoT.Tests.Core.Services
{
  public class SecureKeySuggestServiceTest
  {
    private static SecureKeySuggestService BuildService()
    {
      return new SecureKeySuggestService();
    }

    [Fact]
    public void GenerateSecureKeyWithByteLength_ReturnsKey()
    {
      // arrange
      var service = BuildService();

      // act
      var key = service.GenerateSecureKeyWithByteLength(128);

      // assert
      Assert.NotNull(key);
      Assert.Equal(256, key.Length);
    }

    [Fact]
    public void GenerateSecureKeyWithByteLength_ThrowsIfLargeThanUpperBound()
    {
      var service = BuildService();

      var action = () => service.GenerateSecureKeyWithByteLength(SecureKeySuggestService.ByteLengthUpperBound+1);

      Assert.Throws<ArgumentOutOfRangeException>(action);
    }

    [Fact]
    public void GenerateSecureKeyWithByteLength_ThrowsIfLowerThanLowerBound()
    {
      var service = BuildService();

      var action = () => service.GenerateSecureKeyWithByteLength(SecureKeySuggestService.ByteLengthLowerBound-1);

      Assert.Throws<ArgumentOutOfRangeException>(action);
    }
  }
}
