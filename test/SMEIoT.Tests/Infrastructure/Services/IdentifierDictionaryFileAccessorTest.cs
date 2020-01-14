using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Exceptions;
using SMEIoT.Core.Interfaces;
using SMEIoT.Infrastructure.Data;
using SMEIoT.Infrastructure.Services;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace SMEIoT.Tests.Infrastructure.Services
{
  public class IdentifierDictionaryFileAccessorTest
  {
    private readonly IIdentifierDictionaryFileAccessor _accessor;
    
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
