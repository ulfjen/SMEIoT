using System.IO;
using SMEIoT.Core.Exceptions;
using SMEIoT.Infrastructure.Data;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace SMEIoT.Tests.Infrastructure.Data
{
  public class IdentifierDictionaryFileAccessorTest
  {
    private readonly IdentifierDictionaryFileAccessor _accessor;
    
    public IdentifierDictionaryFileAccessorTest()
    {
      var dir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..");
      _accessor = new IdentifierDictionaryFileAccessor(new PhysicalFileProvider(dir));
    }

    [Fact]
    public void ListIdentifiers_ReturnsWithValidPath()
    {
      // arrange
      
      // act
      var ids = _accessor.ListIdentifiers("test-dict.txt");

      // assert
      Assert.Equal(2, ids.Count);
      Assert.Contains("abc", ids);
      Assert.Contains("123", ids);
    }

    [Fact]
    public void ListIdentifiers_ThrowsWhenNotFound()
    {
      Assert.Throws<InternalException>(() => _accessor.ListIdentifiers("not-exists.txt"));
    }
  }
}
