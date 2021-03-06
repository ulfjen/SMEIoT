using System.IO;
using SMEIoT.Core.Exceptions;
using SMEIoT.Infrastructure.Data;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace SMEIoT.Tests.Infrastructure.Data
{
  public class SystemOneLineFileAccessorTest
  {
    private readonly SystemOneLineFileAccessor _accessor;
    
    public SystemOneLineFileAccessorTest()
    {
      var dir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..");
      _accessor = new SystemOneLineFileAccessor(new PhysicalFileProvider(dir));
    }

    [Fact]
    public void GetLine_ReturnsWithValidPath()
    {
      // arrange
      
      // act
      var line = _accessor.GetLine("test-oneline.txt");

      // assert
      Assert.Contains("abc", line);
    }

    [Fact]
    public void GetLine_ReturnsWithFirstLine()
    {

      var line = _accessor.GetLine("test-dict.txt");

      Assert.Contains("abc", line);
    }

    [Fact]
    public void GetLine_ReturnsNull()
    {
      Assert.Null(_accessor.GetLine("not-exists.txt"));
    }
  }
}
