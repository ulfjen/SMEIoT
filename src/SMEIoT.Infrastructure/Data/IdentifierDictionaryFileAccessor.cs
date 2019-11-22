using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.FileProviders;
using SMEIoT.Core.Interfaces;

namespace SMEIoT.Infrastructure.Data
{
  public class IdentifierDictionaryFileAccessor: IIdentifierDictionaryFileAccessor
  {
    private readonly IFileInfo _fileInfo;
    
    public IdentifierDictionaryFileAccessor(IFileProvider fileProvider)
    {
      _fileInfo = fileProvider.GetFileInfo("identifier-candidates.txt");
    }

    public List<string> ListIdentifiers()
    {
      var res = new List<string>();
      
      if (_fileInfo.Exists)
      {
        using var stream = _fileInfo.CreateReadStream();
        using var reader = new StreamReader(stream);
        while (!reader.EndOfStream)
        {
          try
          {
            var line = reader.ReadLine();
            res.Add(line);
          }
          catch (IOException exception)
          {
            throw new SystemException($"unexpected IO exception while reading identifiers: {exception.Message}");
          }
        }
      }
      else
      {
        throw new SystemException("identifier-candidates.txt can't be found.");
      }

      return res;
    }
  }
}
