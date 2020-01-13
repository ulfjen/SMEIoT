using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.FileProviders;
using SMEIoT.Core.Interfaces;
using SMEIoT.Core.Exceptions;

namespace SMEIoT.Infrastructure.Data
{
  public class IdentifierDictionaryFileAccessor : IIdentifierDictionaryFileAccessor
  {
    private readonly IFileProvider _fileProvider;

    public IdentifierDictionaryFileAccessor(IFileProvider fileProvider)
    {
      _fileProvider = fileProvider;
    }

    public IList<string> ListIdentifiers(string path)
    {
      var fileInfo = _fileProvider.GetFileInfo(path);
      var identifiers = new List<string>();
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
              identifiers.Add(line);
            }
          }
          catch (IOException exception)
          {
            throw new InternalException($"unexpected IO exception while reading identifiers: {exception.Message}");
          }
        }
      }
      else
      {
        throw new InternalException($"{path} can't be found.");
      }

      return identifiers;
    }

  }
}
