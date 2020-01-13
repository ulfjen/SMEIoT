using Microsoft.AspNetCore.Identity;
using SMEIoT.Core.Interfaces;
using SMEIoT.Core.Services;
using SMEIoT.Core.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace SMEIoT.Web.Services
{
  public interface ICommonPasswordValidator : IPasswordValidator<User>
  {
  }

  public class CommonPasswordValidator : ICommonPasswordValidator
  {
    public const string CommonPasswordPath = "common-passwords.txt";

    private readonly IList<string> _forbiddenPasswords;

    public CommonPasswordValidator(IIdentifierDictionaryFileAccessor fileAccessor)
    {
      _forbiddenPasswords = fileAccessor.ListIdentifiers(CommonPasswordPath);
    }
    
    public async Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user, string password)
    {
      var errors = new List<IdentityError>();
      if (_forbiddenPasswords.Contains(password)) {
        errors.Add(UserManagementService.TooCommonPasswordError);
        return IdentityResult.Failed(errors.ToArray());
      }
      return IdentityResult.Success;
    }
  }
}
