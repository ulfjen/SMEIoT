using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Exceptions;
using SMEIoT.Core.Services;
using SMEIoT.Infrastructure.Data;
using SMEIoT.Tests.Shared;
using Xunit;

namespace SMEIoT.Tests.Core.Services
{
  public class UserManagementTest
  {
    private static (UserManagementService, ApplicationDbContext, UserManager<User>) BuildService()
    {
      var um = MockHelpers.CreateUserManager();
      var rm = MockHelpers.CreateRoleManager();
      var dbContext = ApplicationDbContextHelper.BuildTestDbContext();

      return (new UserManagementService(dbContext, um, rm, new NullLogger<UserManagementService>()), dbContext, um);
    }

    [Fact]
    public async Task GetUserAndRoleByName_ThrowsIfNoUser()
    {
      // arrange
      var (service, dbContext, userManager) = BuildService();
      
      // act
      Task Act() => service.GetUserAndRoleByName("not-exist-user");

      // assert
      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var notFound = Assert.IsType<EntityNotFoundException>(exce);
      Assert.Equal("userName", notFound.ParamName);
    }
    
    [Fact]
    public async Task GetUserAndRoleByName_ReturnsUserAndRole()
    {
      var (service, dbContext, userManager) = BuildService();
      const string username = "normal-user";
      await userManager.CreateAsync(new User {UserName = username, ConcurrencyStamp = Guid.NewGuid().ToString()},
        "a-password-1");
      await userManager.AddToRolesAsync(await userManager.FindByNameAsync(username), new[] {"roles1", "roles2"});
      
      var (user, roles) = await service.GetUserAndRoleByName(username);

      Assert.Equal(username, user.UserName);
      Assert.Equal(2, roles.Count);
      Assert.True(await userManager.IsInRoleAsync(user, "roles1"));
      Assert.True(await userManager.IsInRoleAsync(user, "roles2"));
    }
    
    [Fact]
    public async Task CreateUserWithPassword_AssignFirstUserWithAdminRole()
    {
      var (service, dbContext, userManager) = BuildService();
      const string username1 = "normal-username-1";
      const string username2 = "normal-username-2";
      
      await service.CreateUserWithPassword(username1, "a-normal-password-1");
      await service.CreateUserWithPassword(username2, "a-normal-password-2");

      var roles1 = await userManager.GetRolesAsync(await userManager.FindByNameAsync(username1));
      var roles2 = await userManager.GetRolesAsync(await userManager.FindByNameAsync(username2));
      Assert.Equal(1, roles1.Count);
      Assert.True(await userManager.IsInRoleAsync(await userManager.FindByNameAsync(username1), "Admin"));
      Assert.Equal(0, roles2.Count);
    }
    
    [Fact]
    public async Task UpdateUserPassword_ThrowsIfNoUser()
    {
      var (service, dbContext, userManager) = BuildService();
      
      Task Act() => service.GetUserAndRoleByName("not-exist-user");

      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var notFound = Assert.IsType<EntityNotFoundException>(exce);
      Assert.Equal("userName", notFound.ParamName);
    }
    
    [Fact]
    public async Task UpdateUserPassword_UpdatesPassword()
    {
      var (service, dbContext, userManager) = BuildService();
      const string username = "normal-username-1";
      const string password = "a-normal-password";
      const string newPassword = "a-updated-password";
      await userManager.CreateAsync(new User {UserName = username, ConcurrencyStamp = Guid.NewGuid().ToString()},
        password);
      
      var res = await service.UpdateUserPassword(username, password, newPassword);

      Assert.True(res); 
    }

    
    [Fact]
    public async Task UpdateUserRoles_ThrowsIfNoUser()
    {
      var (service, dbContext, userManager) = BuildService();
      var roles = new[] {"Admin", "TestRole1"};

      Task Act() => service.UpdateUserRoles("not-exist-user", roles);

      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var notFound = Assert.IsType<EntityNotFoundException>(exce);
      Assert.Equal("userName", notFound.ParamName);
    }
    
    [Fact]
    public async Task UpdateUserRoles_Returns()
    {
      var (service, dbContext, userManager) = BuildService();
      const string username = "normal-username-1";
      const string password = "a-normal-password";
      var roles = new[] {"Admin", "TestRole1"};
      await userManager.CreateAsync(new User {UserName = username, ConcurrencyStamp = Guid.NewGuid().ToString()},
        password);
      var user = await userManager.FindByNameAsync(username);
      await userManager.AddToRolesAsync(user, new[] {"Admin"});
      
      var res = await service.UpdateUserRoles(username, roles);

      Assert.True(res);
      foreach (var role in roles)
      {
        Assert.True(await userManager.IsInRoleAsync(user, role));
      }
    }

    [Fact]
    public async Task UpdateUserRoles_ClearRoles()
    {
      var (service, dbContext, userManager) = BuildService();
      const string username = "normal-username-1";
      const string password = "a-normal-password";
      await userManager.CreateAsync(new User { UserName = username, ConcurrencyStamp = Guid.NewGuid().ToString() },
        password);
      var user = await userManager.FindByNameAsync(username);
      await userManager.AddToRolesAsync(user, new[] { "Admin" });

      var res = await service.UpdateUserRoles(username, Array.Empty<string>());

      Assert.True(res);
      Assert.False(await userManager.IsInRoleAsync(user, "Admin"));
    }


    [Fact]
    public async Task ListBasicUserResultAsync_ReturnsUserAndRole()
    {
      var (service, dbContext, userManager) = BuildService();
      for (var x = 1; x <= 12; ++x)
      {
        var username = $"normal-user-{x}";
        var user = new User {UserName = username, ConcurrencyStamp = Guid.NewGuid().ToString()};
        dbContext.Users.Add(user);
        // duplicated because of mock
        await userManager.CreateAsync(user,
          "a-password-1");
        await userManager.AddToRolesAsync(await userManager.FindByNameAsync(username), new[] {"roles1", "roles2"}); 
      }
      await dbContext.SaveChangesAsync();
      var res = new List<(User, IList<string>)>();

      await foreach (var (user, roles) in service.ListBasicUserResultAsync(1, 2))
      {
        res.Add((user, roles));
      }

      Assert.Equal(2, res.Count);
      Assert.Contains(res, u => u.Item1.UserName == "normal-user-1" && u.Item2.Contains("ROLES1"));
      Assert.Contains(res, u => u.Item1.UserName == "normal-user-2");
    }

  }
}
