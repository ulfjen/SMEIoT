using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Interfaces;
using SMEIoT.Infrastructure.Data;
using SMEIoT.Web.Services;
using SMEIoT.Web;

namespace SMEIoT.Tests.Shared
{
  public static class MockHelpers
  {
    public static (UserManager, RoleManager<IdentityRole<long>>) CreateUserManager() 
    {
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

      var accessor = new Mock<IIdentifierDictionaryFileAccessor>();
      var forbiddenPasswordList = new List<string>();
      forbiddenPasswordList.Add("zxcvbnm123456789");
      accessor.Setup(a => a.ListIdentifiers(It.IsAny<string>())).Returns(forbiddenPasswordList);

      var roleStore = new InMemoryRoleStore();

      var roleValidators = new List<IRoleValidator<IdentityRole<long>>>();
      roleValidators.Add(new RoleValidator<IdentityRole<long>>());

      var roleManager = new RoleManager<IdentityRole<long>>(roleStore, roleValidators, new UpperInvariantLookupNormalizer(), new IdentityErrorDescriber(), new Mock<ILogger<RoleManager<IdentityRole<long>>>>().Object);
      
      var userManager = new UserManager(new InMemoryUserStore(roleStore), options.Object, new ClearTextPasswordHasher<User>(), 
          userValidators, pwdValidators, new CommonPasswordValidator(accessor.Object), new UpperInvariantLookupNormalizer(),
          new IdentityErrorDescriber(), null,
          new Mock<ILogger<UserManager<User>>>().Object);

      return (userManager, roleManager);
    }
  }
}
