using System;
using SMEIoT.Core.Interfaces;

namespace SMEIoT.Core.Services
{
  public class SecureKeySuggestService : ISecureKeySuggestService
  {
    public const int KeyLengthThreshold = 0;

    public string GenerateSecureKey(int length)
    {
      if (length < KeyLengthThreshold)
      {
        throw new ArgumentException($"Requested a {length} secured key. Use a positive length with certain threshold.");
      }

      using (var rngCsp = new System.Security.Cryptography.RNGCryptoServiceProvider())
      {
        var randomData = new byte[length];
        rngCsp.GetBytes(randomData);
        return Convert.ToBase64String(randomData);
      }
    }
  }
}
