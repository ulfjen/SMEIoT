using System;
using System.IO;
using Microsoft.Extensions.FileProviders;
using SMEIoT.Core.Interfaces;

namespace SMEIoT.Infrastructure.Data
{
  public class SystemOneLineFileAccessor : ISystemSystemOneLineFileAccessor
  {
    private readonly IFileProvider _fileProvider;

    public SystemOneLineFileAccessor(IFileProvider fileProvider)
    {
      _fileProvider = fileProvider;
    }

    public string? GetLine(string path)
    {
      var fileInfo = _fileProvider.GetFileInfo(path);
      if (!fileInfo.Exists) { return null; }
      using var stream = fileInfo.CreateReadStream();
      using var reader = new StreamReader(stream);
      while (!reader.EndOfStream)
      {
        try
        {
          var line = reader.ReadLine();
          if (line != null)
          {
            return line;
          }
        }
        catch (Exception)
        {    
          return null;
        }
      }
      return null;
    }
  }
}
