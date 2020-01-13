using System.Collections.Generic;

namespace SMEIoT.Core.Interfaces
{
  public interface IIdentifierDictionaryFileAccessor
  {
    IList<string> ListIdentifiers(string path);
  }
}
