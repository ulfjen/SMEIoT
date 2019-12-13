using System;
using SMEIoT.Core.Interfaces;
using System.Diagnostics.Contracts; 

namespace SMEIoT.Core.Services
{
  /// handles client psk id / key generation
  public class MosquittoClientAuthenticationService
  {
    // private string? _clientName { get; }
    // private string? _clientPsk { get; }

    // identifier can't be larger than 128 as the RFC. Some implementations might use the final byte for NULL.
    public const int ClientNameLength = 128 - 1;

    public string ClientName { get; }
    //   get {
    //     if (_clientName == null) {
    //       _clientName = LazyInitClientName();
    //     }
    //     return _clientName;
    //   }
    // }

    public string ClientPsk { get; }
    //   get {
    //     if (_clientPsk == null) {
    //       _clientPsk = LazyInitClientPsk();
    //     }
    //     return _clientPsk;
    //   }
    // }

    public MosquittoClientAuthenticationService(ISecureKeySuggestService service)
    {
      // _service = service;
      // ClientName is sent publically. It doesn't matter about the size.
      Contract.Requires(ClientNameLength >= SecureKeySuggestService.ByteLengthLowerBound * 2);
      ClientName = service.GenerateSecureKeyWithByteLength(SecureKeySuggestService.ByteLengthLowerBound);
      ClientPsk = service.GenerateSecureKeyWithByteLength(SecureKeySuggestService.ByteLengthUpperBound);
    }

    // private string LazyInitClientName()
    // {
    //   return _service.GenerateSecureKeyWithByteLength(ClientNameLength);
    // }

    // private string LazyInitClientPsk()
    // {
    //   return _service.GenerateSecureKeyWithByteLength(SecureKeySuggestService.ByteLengthUpperBound - 1);
    // }
  }
}
