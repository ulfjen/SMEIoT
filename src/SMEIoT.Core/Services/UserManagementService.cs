using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Exceptions;
using SMEIoT.Core.Interfaces;
using System.Security.Claims;

namespace SMEIoT.Core.Services
{
  // IdentityResult has an error catelog Identity/Extensions.Core/src/IdentityErrorDescriber.cs
  public class UserManagementService : IUserManagementService
  {
    public const string Entity = "entity";

    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<long>> _roleManager;
    private readonly ILogger _logger;
    private readonly IApplicationDbContext _dbContext;

    public UserManagementService(IApplicationDbContext dbContext,
      UserManager<User> userManager,
      RoleManager<IdentityRole<long>> roleManager,
      ILogger<UserManagementService> logger)
    {
      _dbContext = dbContext;
      _userManager = userManager;
      _roleManager = roleManager;
      _logger = logger;
    }

    public async Task<(User, IList<string>)> GetUserAndRoleByName(string userName)
    {
      var user = await _userManager.FindByNameAsync(userName);
      if (user == null)
      {
        throw new EntityNotFoundException($"cannot find the user {userName}.", "userName");
      }

      var roles = await _userManager.GetRolesAsync(user);
      return (user, roles);
    }

    public async Task<(User, IList<string>)> GetUserAndRoleByPrincipal(ClaimsPrincipal principal)
    {
      var user = await _userManager.GetUserAsync(principal);
      if (user == null)
      {
        throw new EntityNotFoundException($"cannot find the user.", "principal");
      }

      var roles = await _userManager.GetRolesAsync(user);
      return (user, roles);
    }

    public async Task<bool> CreateUserWithPassword(string userName, string password)
    {
      var result =
        await _userManager.CreateAsync(new User {UserName = userName, SecurityStamp = Guid.NewGuid().ToString()},
          password);
      if (!result.Succeeded)
      {
        foreach (var err in result.Errors) {
          switch (err.Code) {
            case nameof(IdentityErrorDescriber.DuplicateUserName):
              throw new InvalidArgumentException("There is something wrong with your userName and password. Try another.", Entity);
            case nameof(IdentityErrorDescriber.PasswordTooShort):
              goto case nameof(IdentityErrorDescriber.PasswordRequiresUpper);
            case nameof(IdentityErrorDescriber.PasswordRequiresUniqueChars):
              goto case nameof(IdentityErrorDescriber.PasswordRequiresUpper);
            case nameof(IdentityErrorDescriber.PasswordRequiresNonAlphanumeric):
              goto case nameof(IdentityErrorDescriber.PasswordRequiresUpper);
            case nameof(IdentityErrorDescriber.PasswordRequiresDigit):
              goto case nameof(IdentityErrorDescriber.PasswordRequiresUpper);
            case nameof(IdentityErrorDescriber.PasswordRequiresLower):
              goto case nameof(IdentityErrorDescriber.PasswordRequiresUpper);
            case nameof(IdentityErrorDescriber.PasswordRequiresUpper):
              throw new InvalidArgumentException(err.Description, "password");
            default:
              throw new InvalidArgumentException(err.Description, Entity);
          }
        }
      }

      var storedUser = await _userManager.FindByNameAsync(userName);

      // depends on storage provider providing a sequence starting from 1
      if (storedUser.Id <= 1L)
      {
        _logger.LogDebug($"Assign new user {storedUser.UserName} with admin role");
        result = await _userManager.AddToRoleAsync(storedUser, "Admin");
        if (!result.Succeeded)
        {
          throw new Exception(result.Errors.Select(e => e.Code).ToString());
        }
      }

      return true;
    }

    public async Task<bool> UpdateUserPassword(string userName, string currentPassword, string newPassword)
    {
      var user = await _userManager.FindByNameAsync(userName);
      if (user == null)
      {
        throw new EntityNotFoundException($"cannot find the user {userName}.", "userName");
      }

      var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
      if (!result.Succeeded)
      {
        if (result.Errors.Any(e => e.Code == "PasswordMismatch"))
        {
          throw new InvalidArgumentException("Password is not correct.", "currentPassword");
        }
        throw new InvalidArgumentException(string.Join(',', result.Errors.ToList().Select(e => e.Description.ToString())), "errors");
      }

      return true;
    }

    public async Task<bool> UpdateUserRoles(string userName, IEnumerable<string> roles)
    {
      var user = await _userManager.FindByNameAsync(userName);
      if (user == null)
      {
        throw new EntityNotFoundException($"cannot find the user {userName}.", "userName");
      }

      if (roles == null)
      {
        throw new EntityNotFoundException($"cannot find the roles.", "roles");
      }

      var roleMap = new Dictionary<string, IdentityRole<long>>();
      var existingRoles = new HashSet<string>();
      foreach (var role in await _userManager.GetRolesAsync(user))
      {
        existingRoles.Add(role);
      }

      var normalizer = _roleManager.KeyNormalizer;
      foreach (var roleName in roles)
      {
        roleMap[normalizer.NormalizeName(roleName)] = await _roleManager.FindByNameAsync(roleName);
      }

      var add = await _userManager.AddToRolesAsync(user, roleMap.Keys.Except(existingRoles));
      if (!add.Succeeded)
      {
        throw new Exception(add.Errors.Select(e=>e.Code).ToString());
      }

      var remove = await _userManager.RemoveFromRolesAsync(user, existingRoles.Except(roleMap.Keys));
      if (!remove.Succeeded)
      {
        throw new Exception(remove.Errors.Select(e=>e.Code).ToString());
      }


      return true;
    }

    public async IAsyncEnumerable<(User, IList<string>)> ListBasicUserResultAsync(int start, int limit)
    {
      if (start <= 0)
      {
        throw new ArgumentException("start can't be negative or zero");
      }
      if (limit < 0)
      {
        throw new ArgumentException("limit can't be negative"); 
      }
      await foreach (var user in _dbContext.Users.OrderBy(u => u.Id).Skip(start-1).Take(limit).AsAsyncEnumerable())
      {
        var roles = await _userManager.GetRolesAsync(user);
        yield return (user, roles);
      }
    }

    public Task<bool> IsAdmin(IEnumerable<string> roles)
    {
      return Task.FromResult(roles.Contains("Admin"));
    }
  }

}
