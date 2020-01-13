using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using SMEIoT.Core.Entities;
using System.Linq;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace SMEIoT.Web.Services
{
  public class UserManager: UserManager<User>
  {
    public UserManager(IUserStore<User> store,
      IOptions<IdentityOptions> optionsAccessor,
      IPasswordHasher<User> passwordHasher,
      IEnumerable<IUserValidator<User>> userValidators,
      IEnumerable<IPasswordValidator<User>> passwordValidators,
      ICommonPasswordValidator commonPasswordValidator,
      ILookupNormalizer keyNormalizer,
      IdentityErrorDescriber errors,
      IServiceProvider services,
      ILogger<UserManager<User>> logger)
      : base(
        store,
        optionsAccessor,
        passwordHasher,
        userValidators,
        passwordValidators.Concat(new IPasswordValidator<User>[] { commonPasswordValidator }),
        keyNormalizer,
        errors,
        services,
        logger
      )
    {
    }

  }
}
