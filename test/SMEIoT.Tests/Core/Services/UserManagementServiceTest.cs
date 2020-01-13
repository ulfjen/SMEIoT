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
using SMEIoT.Web.Services;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace SMEIoT.Tests.Core.Services
{
  public class UserManagementServiceTest: IDisposable
  {
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager _userManager;
    private readonly RoleManager<IdentityRole<long>> _roleManager;
    private UserManagementService _service;

    public UserManagementServiceTest()
    {
      _dbContext = ApplicationDbContextHelper.BuildTestDbContext();
      _userManager = IdentityHelpers.BuildUserManager(ApplicationDbContextHelper.BuildTestDbContext());
      _roleManager = IdentityHelpers.BuildRoleManager(ApplicationDbContextHelper.BuildTestDbContext());
      _service = new UserManagementService(_dbContext, _userManager, _roleManager, new NullLogger<UserManagementService>());
    }
    
    public void Dispose()
    {
      _dbContext.Database.ExecuteSqlInterpolated($"TRUNCATE user_roles, user_tokens, user_logins, user_claims, role_claims, roles, users RESTART IDENTITY CASCADE;");
      _dbContext.Dispose();
    }
    
    private async Task SeedDefaultRolesAsync()
    {
      foreach (var r in new[] {"roles1", "roles2", "roles3", "Admin"})
      {
        await _roleManager.CreateAsync(new IdentityRole<long>(r));
      }
    }

    [Fact]
    public async Task GetUserAndRoleByNameAsync_ThrowsIfNoUser()
    {
      // arrange
      
      // act
      Task Act() => _service.GetUserAndRoleByNameAsync("not-exist-user");

      // assert
      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var details = Assert.IsType<EntityNotFoundException>(exce);
      Assert.Equal("userName", details.ParamName);
    }
    
    [Fact]
    public async Task GetUserAndRoleByNameAsync_ReturnsUserAndRole()
    {
      await SeedDefaultRolesAsync();
      const string userName = "normal-user";
      await _userManager.CreateAsync(new User {UserName = userName, ConcurrencyStamp = Guid.NewGuid().ToString()}, "a-password-1");
      await _userManager.AddToRolesAsync(await _userManager.FindByNameAsync(userName), new[] {"roles1", "roles2"});
      
      var (user, roles) = await _service.GetUserAndRoleByNameAsync(userName);

      Assert.Equal(userName, user.UserName);
      Assert.Equal(2, roles.Count);
      Assert.True(await _userManager.IsInRoleAsync(user, "roles1"));
      Assert.True(await _userManager.IsInRoleAsync(user, "roles2"));
    }
    
    [Fact]
    public async Task CreateUserWithPasswordAsync_AssignFirstUserWithAdminRole()
    {
      const string userName1 = "normal-userName-1";
      const string userName2 = "normal-userName-2";
      
      await _service.CreateUserWithPasswordAsync(userName1, "a-normal-password-1");
      await _service.CreateUserWithPasswordAsync(userName2, "a-normal-password-2");

      var roles1 = await _userManager.GetRolesAsync(await _userManager.FindByNameAsync(userName1));
      var roles2 = await _userManager.GetRolesAsync(await _userManager.FindByNameAsync(userName2));
      Assert.Equal(1, roles1.Count);
      Assert.True(await _userManager.IsInRoleAsync(await _userManager.FindByNameAsync(userName1), "Admin"));
      Assert.Equal(0, roles2.Count);
    }

    [Fact]
    public async Task CreateUserWithPasswordAsync_ThrowsWhenDuplicateUser()
    {
      const string userName1 = "normal-userName-1";
      await _service.CreateUserWithPasswordAsync(userName1, "a-normal-password-1");
      
      Task Act() => _service.CreateUserWithPasswordAsync(userName1, "a-normal-password-2");

      var res = await Record.ExceptionAsync(Act);
      var details = Assert.IsType<InvalidUserInputException>(res);
      Assert.Contains("username or password", details.Message);
    }
   
    [Fact]
    public async Task CreateUserWithPasswordAsync_ThrowsWhenPasswordTooShort()
    {
      const string userName1 = "normal-userName-1";
      
      Task Act() => _service.CreateUserWithPasswordAsync(userName1, "1short");

      var res = await Record.ExceptionAsync(Act);
      var exce = Assert.IsType<InvalidArgumentException>(res);
      Assert.Equal("password", exce.ParamName);
      Assert.Contains("at least", exce.Message);
    }

    [Fact]
    public async Task CreateUserWithPasswordAsync_ThrowsWhenRequiredUniqueCharsNotMet()
    {
      const string userName1 = "normal-userName-1";
      
      Task Act() => _service.CreateUserWithPasswordAsync(userName1, "pppppppppppppppp");

      var res = await Record.ExceptionAsync(Act);
      var exce = Assert.IsType<InvalidArgumentException>(res);
      Assert.Equal("password", exce.ParamName);
      Assert.Contains("different characters", exce.Message);
    }

    [Fact]
    public async Task CreateUserWithPasswordAsync_ThrowsWhenTooCommon()
    {
      const string userName1 = "normal-userName-1";
      
      Task Act() => _service.CreateUserWithPasswordAsync(userName1, "zxcvbnm123456789");

      var res = await Record.ExceptionAsync(Act);
      var exce = Assert.IsType<InvalidArgumentException>(res);
      Assert.Equal("password", exce.ParamName);
      Assert.Contains("common", exce.Message);
    }

    [Fact]
    public async Task UpdateUserPasswordAsync_ThrowsIfNoUser()
    {
      
      Task Act() => _service.GetUserAndRoleByNameAsync("not-exist-user");

      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var notFound = Assert.IsType<EntityNotFoundException>(exce);
      Assert.Equal("userName", notFound.ParamName);
    }
    
    [Fact]
    public async Task UpdateUserPasswordAsync_UpdatesPassword()
    {
      const string userName = "normal-userName-1";
      const string password = "a-normal-password";
      const string newPassword = "a-updated-password";
      await _userManager.CreateAsync(new User {UserName = userName, ConcurrencyStamp = Guid.NewGuid().ToString()},
        password);
      
      Task Act() => _service.UpdateUserPasswordAsync(userName, password, newPassword);

      var exce = await Record.ExceptionAsync(Act);
      Assert.Null(exce);
      var user = await _userManager.FindByNameAsync(userName);
      Assert.True(await _userManager.CheckPasswordAsync(user, newPassword));
    }

    [Fact]
    public async Task UpdateUserPasswordAsync_ThrowsMismatchedPassword()
    {
      const string userName = "normal-userName-1";
      const string password = "a-normal-password";
      const string newPassword = "a-updated-password";
      await _userManager.CreateAsync(new User {UserName = userName, ConcurrencyStamp = Guid.NewGuid().ToString()},
        password);
      
      Task Act() => _service.UpdateUserPasswordAsync(userName, "what's the password", newPassword);

      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var details = Assert.IsType<InvalidArgumentException>(exce);
      Assert.Equal("password", details.ParamName);
    }

    [Fact]
    public async Task UpdateUserPasswordAsync_ThrowsWhenPasswordTooShort()
    {
      const string userName = "normal-userName-1";
      const string password = "a-normal-password";
      const string newPassword = "1short";
      await _userManager.CreateAsync(new User {UserName = userName, ConcurrencyStamp = Guid.NewGuid().ToString()},
        password);
      
      Task Act() => _service.UpdateUserPasswordAsync(userName, password, newPassword);

      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var details = Assert.IsType<InvalidArgumentException>(exce);
      Assert.Equal("newPassword", details.ParamName);
      Assert.Contains("at least", details.Message);
    }

    [Fact]
    public async Task UpdateUserPasswordAsync_ThrowsWhenRequiredUniqueCharsNotMet()
    {
      const string userName = "normal-userName-1";
      const string password = "a-normal-password";
      const string newPassword = "pppppppppppppppppppp";
      await _userManager.CreateAsync(new User {UserName = userName, ConcurrencyStamp = Guid.NewGuid().ToString()},
        password);
      
      Task Act() => _service.UpdateUserPasswordAsync(userName, password, newPassword);

      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var details = Assert.IsType<InvalidArgumentException>(exce);
      Assert.Equal("newPassword", details.ParamName);
      Assert.Contains("different characters", details.Message);
    }

    [Fact]
    public async Task UpdateUserPasswordAsync_ThrowsWhenTooCommon()
    {
      const string userName = "normal-userName-1";
      const string password = "a-normal-password";
      const string newPassword = "zxcvbnm123456789";
      await _userManager.CreateAsync(new User {UserName = userName, ConcurrencyStamp = Guid.NewGuid().ToString()},
        password);
      
      Task Act() => _service.UpdateUserPasswordAsync(userName, password, newPassword);

      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var details = Assert.IsType<InvalidArgumentException>(exce);
      Assert.Equal("newPassword", details.ParamName);
      Assert.Contains("common", details.Message);
    }
    
    [Fact]
    public async Task UpdateUserRolesAsync_ThrowsIfNoUser()
    {
      var roles = new[] {"Admin", "TestRole1"};

      Task Act() => _service.UpdateUserRolesAsync("not-exist-user", roles);

      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var notFound = Assert.IsType<EntityNotFoundException>(exce);
      Assert.Equal("userName", notFound.ParamName);
    }
    
    
    [Fact]
    public async Task UpdateUserRolesAsync_ThrowsIfAnyRoleIsNull()
    {
      const string userName = "normal-userName-1";
      const string password = "a-normal-password";
      await _userManager.CreateAsync(new User {UserName = userName, ConcurrencyStamp = Guid.NewGuid().ToString()},
        password);
      var user = await _userManager.FindByNameAsync(userName);
      var roles = new[] {"Admin", null, "TestRole1"};

      Task Act() => _service.UpdateUserRolesAsync(user.UserName, roles);

      var exce = await Record.ExceptionAsync(Act);
      Assert.NotNull(exce);
      var details = Assert.IsType<InvalidArgumentException>(exce);
      Assert.Equal("roles", details.ParamName);
    }
    
    [Fact]
    public async Task UpdateUserRolesAsync_UpdateNewRole()
    {
      await SeedDefaultRolesAsync();
      
      const string userName = "normal-userName-1";
      const string password = "a-normal-password";
      var roles = new[] {"Admin", "roles1"};
      await _userManager.CreateAsync(new User {UserName = userName, ConcurrencyStamp = Guid.NewGuid().ToString()},
        password);
      var user = await _userManager.FindByNameAsync(userName);
      await _userManager.AddToRolesAsync(user, new[] {"roles1"});
      
      await _service.UpdateUserRolesAsync(userName, roles);

      foreach (var role in roles)
      {
        Assert.True(await _userManager.IsInRoleAsync(user, role));
      }
    }
    
    [Fact]
    public async Task UpdateUserRolesAsync_ReplaceRoles()
    {
      const string userName = "normal-userName-1";
      const string password = "a-normal-password";
      var roles = new[] {"AnotherRole", "TestRole1"};
      foreach (var r in roles)
      {
        await _roleManager.CreateAsync(new IdentityRole<long>(r));
      }
      await _userManager.CreateAsync(new User {UserName = userName, ConcurrencyStamp = Guid.NewGuid().ToString()},
        password);
      var user = await _userManager.FindByNameAsync(userName);
      await _userManager.AddToRolesAsync(user, new[] {"Admin"});
      
      await _service.UpdateUserRolesAsync(userName, roles);

      foreach (var role in roles)
      {
        Assert.True(await _userManager.IsInRoleAsync(user, role));
      }

      Assert.False(await _userManager.IsInRoleAsync(user, "Admin"));
    }

    [Fact]
    public async Task UpdateUserRolesAsync_ClearRolesWithEmpty()
    {
      const string userName = "normal-userName-1";
      const string password = "a-normal-password";
      await _userManager.CreateAsync(new User { UserName = userName, ConcurrencyStamp = Guid.NewGuid().ToString() },
        password);
      var user = await _userManager.FindByNameAsync(userName);
      await _userManager.AddToRolesAsync(user, new[] { "Admin" });

      await _service.UpdateUserRolesAsync(userName, Array.Empty<string>());

      Assert.False(await _userManager.IsInRoleAsync(user, "Admin"));
    }

    [Fact]
    public async Task UpdateUserRolesAsync_ClearRolesWithNull()
    {
      const string userName = "normal-userName-1";
      const string password = "a-normal-password";
      await _userManager.CreateAsync(new User { UserName = userName, ConcurrencyStamp = Guid.NewGuid().ToString() },
        password);
      var user = await _userManager.FindByNameAsync(userName);
      await _userManager.AddToRolesAsync(user, new[] { "Admin" });

      await _service.UpdateUserRolesAsync(userName, null);

      Assert.False(await _userManager.IsInRoleAsync(user, "Admin"));
    }

    [Fact]
    public async Task IsAdminAsync_FalseWhenNull()
    {
      
      var res = await _service.IsAdminAsync(null);

      Assert.False(res);
    }

    [Fact]
    public async Task IsAdminAsync_ReturnsTrue()
    {
      
      var res = await _service.IsAdminAsync(new[] { "Admin" });

      Assert.True(res);
    }

    [Fact]
    public async Task IsAdminAsync_ReturnsFalse()
    {
      
      var res = await _service.IsAdminAsync(new[] { "NAdmin" });

      Assert.False(res);
    }

    [Fact]
    public async Task IsAdminAsync_ReturnsTrueWhenSymbolIsNotNormalized()
    {
      
      var res = await _service.IsAdminAsync(new[] { "adMin" });

      Assert.True(res);
    }
    
    [Fact]
    public async Task ListBasicUserResultAsync_AllUsersWhenRoleIsNull()
    {
      await SeedDefaultRolesAsync();
      for (var x = 1; x <= 12; ++x)
      {
        var userName = $"normal-user-{x}";
        var user = new User {UserName = userName, ConcurrencyStamp = Guid.NewGuid().ToString()};
        
        await _userManager.CreateAsync(user,
          "a-password-1");
        await _userManager.AddToRolesAsync(await _userManager.FindByNameAsync(userName), new[] {"roles1", "roles2"}); 
      }
      await _dbContext.SaveChangesAsync();
      var res = new List<(User, IList<string>)>();

      await foreach (var (user, roles) in _service.ListBasicUserResultAsync(0, 20, null))
      {
        res.Add((user, roles));
      }

      Assert.Equal(12, res.Count);
    }
    
    
    [Fact]
    public async Task ListBasicUserResultAsync_ThrowWhenSomeRoleIsNull()
    {
      await SeedDefaultRolesAsync();

      for (var x = 1; x <= 12; ++x)
      {
        var userName = $"normal-user-{x}";
        var user = new User {UserName = userName, ConcurrencyStamp = Guid.NewGuid().ToString()};
        
        await _userManager.CreateAsync(user,
          "a-password-1");
        await _userManager.AddToRolesAsync(await _userManager.FindByNameAsync(userName), new[] {"roles1", "roles2"}); 
      }
      await _dbContext.SaveChangesAsync();
      var res = new List<(User, IList<string>)>();

      var details = await Assert.ThrowsAsync<InvalidArgumentException>(async () =>
      {
        await foreach (var (u, r) in _service.ListBasicUserResultAsync(0, 20, new[] {"roles1", null, "roles2"}))
        {
        }
      });
      Assert.Equal("roles", details.ParamName);
    }


    [Fact]
    public async Task ListBasicUserResultAsync_ReturnsUserAndRoles()
    {
      await SeedDefaultRolesAsync();

      for (var x = 1; x <= 10; ++x)
      {
        var userName = $"normal-user-{x}";
        var user = new User {UserName = userName, ConcurrencyStamp = Guid.NewGuid().ToString()};
        await _userManager.CreateAsync(user,"a-password-1");
        await _userManager.AddToRolesAsync(await _userManager.FindByNameAsync(userName), new[] {"roles1", "roles2"}); 
      }
      for (var x = 11; x <= 12; ++x)
      {
        var userName = $"user-{x}";
        var user = new User {UserName = userName, ConcurrencyStamp = Guid.NewGuid().ToString()};
        await _userManager.CreateAsync(user,"a-password-1");
        await _userManager.AddToRolesAsync(await _userManager.FindByNameAsync(userName), new[] {"roles1", "roles3"}); 
      }
      await _dbContext.SaveChangesAsync();
      var res = new List<(User, IList<string>)>();

      await foreach (var (user, roles) in _service.ListBasicUserResultAsync(0, 20))
      {
        res.Add((user, roles));
      }

      Assert.Equal(12, res.Count);
      Assert.Contains(res, u => u.Item1.UserName == "normal-user-1" && u.Item2.Contains("roles1"));
      Assert.Contains(res, u => u.Item1.UserName == "normal-user-2");
    }

    [Fact]
    public async Task ListBasicUserResultAsync_ReturnsUserAndRolesWithFilteringRole()
    {
      await SeedDefaultRolesAsync();

      for (var x = 1; x <= 10; ++x)
      {
        var userName = $"normal-user-{x}";
        var user = new User {UserName = userName, ConcurrencyStamp = Guid.NewGuid().ToString()};
        await _userManager.CreateAsync(user,"a-password-1");
        await _userManager.AddToRolesAsync(await _userManager.FindByNameAsync(userName), new[] {"roles1", "roles2" }); 
      }
      for (var x = 11; x <= 12; ++x)
      {
        var userName = $"user-{x}";
        var user = new User {UserName = userName, ConcurrencyStamp = Guid.NewGuid().ToString()};
        await _userManager.CreateAsync(user,"a-password-1");
        await _userManager.AddToRoleAsync(await _userManager.FindByNameAsync(userName), "roles3"); 
      }
      await _dbContext.SaveChangesAsync();
      var res = new List<(User, IList<string>)>();

      await foreach (var (user, roles) in _service.ListBasicUserResultAsync(0, 20, new [] { "roles3" }))
      {
        res.Add((user, roles));
      }

      Assert.Equal(2, res.Count);
      Assert.Contains(res, u => u.Item1.UserName == "user-11" && u.Item2.Contains("roles3"));
      Assert.Contains(res, u => u.Item1.UserName == "user-12");
    }

    [Fact]
    public async Task ListBasicUserResultAsync_ThrowsNegativeOffset()
    {
      await SeedDefaultRolesAsync();

      for (var x = 1; x <= 12; ++x)
      {
        var userName = $"normal-user-{x}";
        var user = new User {UserName = userName, ConcurrencyStamp = Guid.NewGuid().ToString()};
        await _userManager.CreateAsync(user,"a-password-1");
        await _userManager.AddToRolesAsync(await _userManager.FindByNameAsync(userName), new[] {"roles1", "roles2"}); 
      }
      await _dbContext.SaveChangesAsync();
      
      var details = await Assert.ThrowsAsync<InvalidArgumentException>(async () =>
      {
        await foreach (var (u, r) in _service.ListBasicUserResultAsync(-1, 20))
        {
        }
      });
      Assert.Equal("offset", details.ParamName);
    }

    [Fact]
    public async Task ListBasicUserResultAsync_AllowZeroOffset()
    {
      await SeedDefaultRolesAsync();

      for (var x = 1; x <= 12; ++x)
      {
        var userName = $"normal-user-{x}";
        var user = new User {UserName = userName, ConcurrencyStamp = Guid.NewGuid().ToString()};
        await _userManager.CreateAsync(user,"a-password-1");
        await _userManager.AddToRolesAsync(await _userManager.FindByNameAsync(userName), new[] {"roles1", "roles2"}); 
      }
      await _dbContext.SaveChangesAsync();
      var res = new List<(User, IList<string>)>();

      await foreach (var (user, roles) in _service.ListBasicUserResultAsync(0, 12))
      {
        res.Add((user, roles));
      }

      Assert.Equal(12, res.Count);
    }
    
    
    [Fact]
    public async Task ListBasicUserResultAsync_ThrowsNegativeLimit()
    {
      await SeedDefaultRolesAsync();

      for (var x = 1; x <= 12; ++x)
      {
        var userName = $"normal-user-{x}";
        var user = new User {UserName = userName, ConcurrencyStamp = Guid.NewGuid().ToString()};
        await _userManager.CreateAsync(user,"a-password-1");
        await _userManager.AddToRolesAsync(await _userManager.FindByNameAsync(userName), new[] {"roles1", "roles2"}); 
      }
      await _dbContext.SaveChangesAsync();
      
      var details = await Assert.ThrowsAsync<InvalidArgumentException>(async () =>
      {
        await foreach (var (u, r) in _service.ListBasicUserResultAsync(1, -1))
        {
        }
      });
      Assert.Equal("limit", details.ParamName);
    }

    [Fact]
    public async Task ListBasicUserResultAsync_AllowZeroLimit()
    {
      await SeedDefaultRolesAsync();

      for (var x = 1; x <= 12; ++x)
      {
        var userName = $"normal-user-{x}";
        var user = new User {UserName = userName, ConcurrencyStamp = Guid.NewGuid().ToString()};
        await _userManager.CreateAsync(user,"a-password-1");
        await _userManager.AddToRolesAsync(await _userManager.FindByNameAsync(userName), new[] {"roles1", "roles2"}); 
      }
      await _dbContext.SaveChangesAsync();
      var res = new List<(User, IList<string>)>();

      await foreach (var (user, roles) in _service.ListBasicUserResultAsync(0, 0))
      {
        res.Add((user, roles));
      }

      Assert.Empty(res);
    }
    
    [Fact]
    public async Task ListBasicUserResultAsync_ReturnsUserAndRolesWithOffset()
    {
      await SeedDefaultRolesAsync();

      for (var x = 1; x <= 12; ++x)
      {
        var userName = $"normal-user-{x}";
        var user = new User {UserName = userName, ConcurrencyStamp = Guid.NewGuid().ToString()};
        await _userManager.CreateAsync(user,"a-password-1");
        user = await _userManager.FindByNameAsync(userName);
        await _userManager.AddToRolesAsync(user, new[] {"roles1", "roles2"}); 
      }
      await _dbContext.SaveChangesAsync();
      var res = new List<(User, IList<string>)>();

      await foreach (var (user, roles) in _service.ListBasicUserResultAsync(10, 12))
      {
        res.Add((user, roles));
      }

      Assert.Equal(2, res.Count);
      Assert.Contains(res, u => u.Item1.UserName == "normal-user-11" && u.Item2.Contains("roles1"));
      Assert.Contains(res, u => u.Item1.UserName == "normal-user-12");
    }

  }
}
