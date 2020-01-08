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
    public void GenerateSecureKey_ReturnsKey()
    {
      // arrange
      var service = BuildService();

      // act
      var key = service.GenerateSecureKey(128);

      // assert
      Assert.NotNull(key);
      Assert.Equal(172, key.Length);
    }
  }
}
