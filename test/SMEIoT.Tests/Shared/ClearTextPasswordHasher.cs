using Microsoft.AspNetCore.Identity;

namespace SMEIoT.Tests.Shared
{
  public class ClearTextPasswordHasher<TUser> : IPasswordHasher<TUser> where TUser : class
  {
    public string HashPassword(TUser user, string password)
    {
      return password;
    }

    public PasswordVerificationResult VerifyHashedPassword(TUser user, string hashedPassword, string providedPassword)
    {
      return hashedPassword == providedPassword ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
    }
  }
}
