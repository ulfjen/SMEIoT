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

    public async Task CreateUserWithPassword(string userName, string password)
    {
      var result =
        await _userManager.CreateAsync(new User {UserName = userName, SecurityStamp = Guid.NewGuid().ToString()},
          password);
      if (!result.Succeeded)
      {
        var passwordErrors = new List<string>();
        foreach (var err in result.Errors) {
          switch (err.Code) {
            case nameof(IdentityErrorDescriber.DuplicateUserName):
              throw new InvalidUserInputException("There is something wrong with your username or password. Try another combination.");
            case nameof(IdentityErrorDescriber.PasswordTooShort):
              passwordErrors.Append(err.Description);
              break;
            case nameof(IdentityErrorDescriber.PasswordRequiresUniqueChars):
              passwordErrors.Append(err.Description);
              break;
            case nameof(IdentityErrorDescriber.PasswordRequiresNonAlphanumeric):
              passwordErrors.Append(err.Description);
              break;
            case nameof(IdentityErrorDescriber.PasswordRequiresDigit):
              passwordErrors.Append(err.Description);
              break;
            case nameof(IdentityErrorDescriber.PasswordRequiresLower):
              passwordErrors.Append(err.Description);
              break;
            case nameof(IdentityErrorDescriber.PasswordRequiresUpper):
              passwordErrors.Append(err.Description);
              break;
            default:
              throw new InvalidUserInputException(err.Description);
          }
        }
        if (passwordErrors.Count != 0) {
          throw new InvalidArgumentException(string.Join('\n', passwordErrors), "password");
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
          HandleIdentityRoleErrors(result.Errors);
        }
      }
    }
    
    private void HandleIdentityRoleErrors(IEnumerable<IdentityError> errors)
    {
      foreach (var err in errors) {
        switch (err.Code) {
          case nameof(IdentityErrorDescriber.InvalidRoleName):
            goto case nameof(IdentityErrorDescriber.UserAlreadyInRole);
          case nameof(IdentityErrorDescriber.DuplicateRoleName):
            goto case nameof(IdentityErrorDescriber.UserAlreadyInRole);
          case nameof(IdentityErrorDescriber.UserNotInRole):
            goto case nameof(IdentityErrorDescriber.UserAlreadyInRole);
          case nameof(IdentityErrorDescriber.UserAlreadyInRole):
            throw new InternalException("Unable to set the first user as admin. Try reinstall.");
          default:
            throw new InternalException(err.Description);
        }
      }
    }

    public async Task UpdateUserPassword(string userName, string currentPassword, string newPassword)
    {
      var user = await _userManager.FindByNameAsync(userName);
      if (user == null)
      {
        throw new EntityNotFoundException($"cannot find the user {userName}.", "userName");
      }

      var passwordErrors = new List<string>();
      var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
      if (!result.Succeeded)
      {
        foreach (var err in result.Errors) {
          switch (err.Code) {
            case nameof(IdentityErrorDescriber.PasswordMismatch):
              throw new InvalidArgumentException(err.Description, "password");
            case nameof(IdentityErrorDescriber.PasswordTooShort):
              passwordErrors.Append(err.Description);
              break;
            case nameof(IdentityErrorDescriber.PasswordRequiresUniqueChars):
              passwordErrors.Append(err.Description);
              break;
            case nameof(IdentityErrorDescriber.PasswordRequiresNonAlphanumeric):
              passwordErrors.Append(err.Description);
              break;
            case nameof(IdentityErrorDescriber.PasswordRequiresDigit):
              passwordErrors.Append(err.Description);
              break;
            case nameof(IdentityErrorDescriber.PasswordRequiresLower):
              passwordErrors.Append(err.Description);
              break;
            case nameof(IdentityErrorDescriber.PasswordRequiresUpper):
              passwordErrors.Append(err.Description);
              break;
            default:
              throw new InvalidUserInputException("Unhandled user password error.");
          }
        }
        if (passwordErrors.Count != 0) {
          throw new InvalidArgumentException(string.Join('\n', passwordErrors), "newPassword");
        }
      }
    }

    public async Task UpdateUserRoles(string userName, IEnumerable<string> roles)
    {
      var user = await _userManager.FindByNameAsync(userName);
      if (user == null)
      {
        throw new EntityNotFoundException($"cannot find the user {userName}.", "userName");
      }

      if (roles == null)
      {
        throw new InvalidArgumentException("cannot find the roles.", "roles");
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
        var role = await _roleManager.FindByNameAsync(roleName);
        if (role == null) {
          continue;
        }
        roleMap[normalizer.NormalizeName(roleName)] = role;
      }

      var result = await _userManager.AddToRolesAsync(user, roleMap.Keys.Except(existingRoles));
      if (!result.Succeeded)
      {       
        HandleIdentityRoleErrors(result.Errors);
      }

      result = await _userManager.RemoveFromRolesAsync(user, existingRoles.Except(roleMap.Keys));
      if (!result.Succeeded)
      {
        HandleIdentityRoleErrors(result.Errors);
      }
    }

    public async IAsyncEnumerable<(User, IList<string>)> ListBasicUserResultAsync(int offset, int limit)
    {
      if (offset < 0)
      {
        throw new InvalidArgumentException("Offset can't be negative", "offset");
      }
      if (limit < 0)
      {
        throw new InvalidArgumentException("limit can't be negative", "limit"); 
      }
      await foreach (var user in _dbContext.Users.OrderBy(u => u.Id).Skip(offset).Take(limit).AsAsyncEnumerable())
      {
        var roles = await _userManager.GetRolesAsync(user);
        yield return (user, roles);
      }
    }

    public async Task<int> NumberOfUsersAsync()
    {
      return await _dbContext.Users.CountAsync();
    }

    public Task<bool> IsAdmin(IEnumerable<string> roles)
    {
      return Task.FromResult(roles.Contains("Admin"));
    }
  }

}
