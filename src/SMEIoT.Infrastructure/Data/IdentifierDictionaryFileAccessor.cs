using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.FileProviders;
using SMEIoT.Core.Interfaces;

namespace SMEIoT.Infrastructure.Data
{
  public class IdentifierDictionaryFileAccessor : IIdentifierDictionaryFileAccessor
  {
    private readonly List<string> _identifiers;

    public IdentifierDictionaryFileAccessor(IFileProvider fileProvider)
    {
      var fileInfo = fileProvider.GetFileInfo("identifier-candidates.txt");

      _identifiers = new List<string>();
      if (fileInfo.Exists)
      {
        using var stream = fileInfo.CreateReadStream();
        using var reader = new StreamReader(stream);
        while (!reader.EndOfStream)
        {
          try
          {
            var line = reader.ReadLine();
            if (line != null)
            {
              _identifiers.Add(line);
            }
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

    }

    public List<string> ListIdentifiers()
    {
      return _identifiers;
    }

  }
}
