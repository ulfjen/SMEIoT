using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SMEIoT.Core.Services;
using Xunit;

namespace SMEIoT.Tests.Core.Services
{
  public class MosquittoClientAuthenticationServiceTest
  {
    private readonly MosquittoClientAuthenticationService _service;

    public MosquittoClientAuthenticationServiceTest()
    {
      var suggestion = new SecureKeySuggestionService();
      _service = new MosquittoClientAuthenticationService(suggestion);
    }

    [Fact]
    public async Task GetClientNameAsync_GetsOneName()
    {
      var name = await _service.GetClientNameAsync();

      Assert.True(name.Length >= 64 && name.Length <= 128);
    }

    [Fact]
    public async Task GetClientNameAsync_GetsSameNameFromAnotherRequest()
    {
      var name = await _service.GetClientNameAsync();

      var name1 = await _service.GetClientNameAsync();

      Assert.Equal(name, name1);
    }

    [Fact]
    public async Task GetClientPskAsync_GetsOneKey()
    {
      var psk = await _service.GetClientPskAsync();

      Assert.Equal(SecureKeySuggestionService.ByteLengthUpperBound * 2, psk.Length);
    }

    [Fact]
    public async Task GetClientNameAsync_GetsSameKey()
    {
      var psk = await _service.GetClientPskAsync();

      var psk1 = await _service.GetClientPskAsync();

      Assert.Equal(psk, psk1);
    }

  }
}
