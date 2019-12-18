using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Interfaces;
using SMEIoT.Web;

namespace SMEIoT.Tests.Shared
{
  public static class MockHelpers
  {
    public static UserManager<User> CreateUserManager() 
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
      var userManager = new UserManager<User>(new InMemoryUserStore(), options.Object, new ClearTextPasswordHasher<User>(), 
          userValidators, pwdValidators, new UpperInvariantLookupNormalizer(),
          new IdentityErrorDescriber(), null,
          new Mock<ILogger<UserManager<User>>>().Object);

      return userManager;
    }

    public static RoleManager<IdentityRole<long>> CreateRoleManager()
    {
      var roleStore = new InMemoryRoleStore();

      var roleValidators = new List<IRoleValidator<IdentityRole<long>>>();
      roleValidators.Add(new RoleValidator<IdentityRole<long>>());

      return new RoleManager<IdentityRole<long>>(roleStore, roleValidators, new UpperInvariantLookupNormalizer(), new IdentityErrorDescriber(), new Mock<ILogger<RoleManager<IdentityRole<long>>>>().Object);
    }
}
}
