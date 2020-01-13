using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using SMEIoT.Core.Entities;
using SMEIoT.Infrastructure.Data;
using SMEIoT.Web;
using SMEIoT.Web.Services;

namespace SMEIoT.Tests.Shared
{
  public static class IdentityHelpers
  {
    public static UserManager BuildUserManager(ApplicationDbContext dbContext)
    {
      var store = new UserStore<User, IdentityRole<long>, ApplicationDbContext, long, IdentityUserClaim<long>, IdentityUserRole<long>, IdentityUserLogin<long>, IdentityUserToken<long>, IdentityRoleClaim<long>>(dbContext, new IdentityErrorDescriber());

      var dir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..");
      var accessor = new IdentifierDictionaryFileAccessor(new PhysicalFileProvider(dir));

      var options = new Mock<IOptions<IdentityOptions>>();
      var idOptions = new IdentityOptions();

      StartupConfigureIdentity.ConfigureIdentityOptions(idOptions);

      options.Setup(o => o.Value).Returns(idOptions);
      
      var userValidators = new List<IUserValidator<User>>();
      var validator = new UserValidator<User>();
      userValidators.Add(validator);

      var passValidator = new PasswordValidator<User>();
      var pwdValidators = new List<IPasswordValidator<User>>();
      pwdValidators.Add(passValidator);

      var userManager = new UserManager(store, options.Object, new ClearTextPasswordHasher<User>(), 
          userValidators, pwdValidators, new CommonPasswordValidator(accessor), new UpperInvariantLookupNormalizer(),
          new IdentityErrorDescriber(), null,
          new NullLogger<UserManager>());
          
      return userManager;
    }

    public static RoleManager<IdentityRole<long>> BuildRoleManager(ApplicationDbContext dbContext)
    {
      var store = new RoleStore<IdentityRole<long>, ApplicationDbContext, long, IdentityUserRole<long>, IdentityRoleClaim<long>>(dbContext);
      
      var roleValidators = new List<IRoleValidator<IdentityRole<long>>>();
      roleValidators.Add(new RoleValidator<IdentityRole<long>>());

      var roleManager = new RoleManager<IdentityRole<long>>(store, roleValidators, new UpperInvariantLookupNormalizer(), new IdentityErrorDescriber(), new NullLogger<RoleManager<IdentityRole<long>>>());
      StartupIdentityDataInitializer.SeedRoles(roleManager);
      return roleManager;
    }
  }
}
