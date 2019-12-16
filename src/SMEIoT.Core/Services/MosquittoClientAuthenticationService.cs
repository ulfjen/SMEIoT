using System;
using SMEIoT.Core.Interfaces;
using System.Diagnostics.Contracts; 

namespace SMEIoT.Core.Services
{
  /// handles client psk id / key generation
  public class MosquittoClientAuthenticationService : IMosquittoClientAuthenticationService
  {
    // identifier can't be larger than 128 as the RFC. Some implementations might use the final byte for NULL.
    public const int ClientNameLength = 128 - 1;

    public string ClientName { get; }
    public string ClientPsk { get; }

    public MosquittoClientAuthenticationService(ISecureKeySuggestService service)
    {
      // ClientName is sent publically. It doesn't matter about the size. but the str returns are twice as large. Used for convenience reason.
      Contract.Requires(ClientNameLength >= SecureKeySuggestService.ByteLengthLowerBound * 2);
      ClientName = service.GenerateSecureKeyWithByteLength(SecureKeySuggestService.ByteLengthLowerBound);
      ClientPsk = service.GenerateSecureKeyWithByteLength(SecureKeySuggestService.ByteLengthUpperBound);
    }
  }
}
