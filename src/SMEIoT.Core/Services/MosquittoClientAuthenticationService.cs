using System;
using System.Threading.Tasks;
using SMEIoT.Core.Interfaces;
using System.Diagnostics.Contracts; 

namespace SMEIoT.Core.Services
{
  /// handles client psk id / key generation
  public class MosquittoClientAuthenticationService : IMosquittoClientAuthenticationService
  {
    // identifier can't be larger than 128 as the RFC. Some implementations might use the final byte for NULL.
    public const int ClientNameLength = 128 - 1;

    private readonly ISecureKeySuggestionService _service;
    private string? _clientName = null;
    private string? _clientPsk = null;
    
    public async Task<string> GetClientNameAsync()
    {
      if (_clientName == null) {
        _clientName = await _service.GenerateSecureKeyWithByteLengthAsync(SecureKeySuggestionService.ByteLengthLowerBound);
      }
      return _clientName;
    }

    public async Task<string> GetClientPskAsync()
    {
      if (_clientPsk == null) {
        _clientPsk = await _service.GenerateSecureKeyWithByteLengthAsync(SecureKeySuggestionService.ByteLengthUpperBound);
      }
      return _clientPsk;
    }

    public MosquittoClientAuthenticationService(ISecureKeySuggestionService service)
    {
      _service = service;
      // ClientName is sent publicly. It doesn't matter about the size. but the str returns are twice as large. Used for convenience reason.
      Contract.Requires(ClientNameLength >= SecureKeySuggestionService.ByteLengthLowerBound * 2);
    }
  }
}
