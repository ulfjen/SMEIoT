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
using System.Linq.Dynamic.Core;

namespace SMEIoT.Core.Services
{
  // IdentityResult has an error catelog Identity/Extensions.Core/src/IdentityErrorDescriber.cs
  public class UserManagementService : IUserManagementService
  {
    private const string TooCommonPasswordErrorCode = nameof(TooCommonPasswordError);
    public static readonly IdentityError TooCommonPasswordError = new IdentityError
    {
      Code = TooCommonPasswordErrorCode,
      Description = "You are trying to use a common password. Please choose a more complicated combination for the password."
    };

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

    public async Task<(User, IList<string>)> GetUserAndRoleByNameAsync(string userName)
    {
      var user = await _userManager.FindByNameAsync(userName);
      if (user == null)
      {
        throw new EntityNotFoundException($"cannot find the user {userName}.", "userName");
      }

      var roles = await _userManager.GetRolesAsync(user);
      return (user, roles);
    }

    public async Task CreateUserWithPasswordAsync(string userName, string password)
    {
      var result =
        await _userManager.CreateAsync(new User {UserName = userName, SecurityStamp = Guid.NewGuid().ToString()},
          password);
      if (!result.Succeeded)
      {
        var passwordErrors = new List<string>();
        foreach (var err in result.Errors) {
          if (err.Code == nameof(IdentityErrorDescriber.DuplicateUserName))
          {
            throw new InvalidUserInputException(
              "There is something wrong with your username or password. Try another combination.");
          }
          else if (!HandleNewPasswordIdentityError(err, passwordErrors))
          {
              throw new InvalidUserInputException(err.Description);
          }
        }
        if (passwordErrors.Count > 0) {
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
          try {
          　 HandleIdentityRoleErrors(result.Errors);
          }
          catch
          {
            throw new InternalException("Unable to set the first user as admin. Try reinstall.");
          }
        }
      }
    }
    
    private bool HandleNewPasswordIdentityError(IdentityError error, IList<string> errors)
    {
      var added = false;
      switch (error.Code)
      {
        case nameof(IdentityErrorDescriber.PasswordTooShort):
          errors.Add(error.Description);
          added = true;
          break;
        case nameof(IdentityErrorDescriber.PasswordRequiresUniqueChars):
          errors.Add(error.Description);
          added = true;
          break;
        case nameof(IdentityErrorDescriber.PasswordRequiresNonAlphanumeric):
          errors.Add(error.Description);
          added = true;
          break;
        case nameof(IdentityErrorDescriber.PasswordRequiresDigit):
          errors.Add(error.Description);
          added = true;
          break;
        case nameof(IdentityErrorDescriber.PasswordRequiresLower):
          errors.Add(error.Description);
          added = true;
          break;
        case nameof(IdentityErrorDescriber.PasswordRequiresUpper):
          errors.Add(error.Description);
          added = true;
          break;
        case TooCommonPasswordErrorCode:
          errors.Add(error.Description);
          added = true;
          break;
      }

      return added;
    }
    
    private void HandleIdentityRoleErrors(IEnumerable<IdentityError> errors)
    {
      foreach (var err in errors) {
        switch (err.Code) {
          case nameof(IdentityErrorDescriber.InvalidRoleName):
            goto default;
          case nameof(IdentityErrorDescriber.DuplicateRoleName):
            goto default;
          case nameof(IdentityErrorDescriber.UserNotInRole):
            goto default;
          case nameof(IdentityErrorDescriber.UserAlreadyInRole):
            goto default;
          default:
            throw new InternalException(err.Description);
        }
      }
    }

    public async Task UpdateUserPasswordAsync(string userName, string currentPassword, string newPassword)
    {
      if (userName == null) {
        throw new InvalidArgumentException("username can't be null", "userName");
      }
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
          if (err.Code == nameof(IdentityErrorDescriber.PasswordMismatch))
          {
            throw new InvalidArgumentException(err.Description, "password");
          }
          else if (!HandleNewPasswordIdentityError(err, passwordErrors))
          {
            throw new InternalException(err.Description);
          }
        }
      }
      if (passwordErrors.Count > 0) {
        throw new InvalidArgumentException(string.Join('\n', passwordErrors), "newPassword");
      } 
    }

    private List<string> SanitizedRoles(IEnumerable<string?>? roles)
    {
      var sanitized = roles == null ? new List<string?>() : roles.ToList();
      if (sanitized.Exists(r => r == null))
      {
        throw new InvalidArgumentException("A invalid role is found.", "roles");
      }

      return sanitized.Select(s => s!).ToList();
    }

    public async Task UpdateUserRolesAsync(string userName, IEnumerable<string?>? roles = null)
    {
      var user = await _userManager.FindByNameAsync(userName);
      if (user == null)
      {
        throw new EntityNotFoundException($"cannot find the user {userName}.", "userName");
      }

      var sanitized = SanitizedRoles(roles);

      var roleMap = new Dictionary<string, IdentityRole<long>>();
      var existingRoles = new HashSet<string>();
      var normalizer = _roleManager.KeyNormalizer;

      foreach (var role in await _userManager.GetRolesAsync(user))
      {
        existingRoles.Add(normalizer.NormalizeName(role));
      }

      foreach (var roleName in sanitized)
      {
        var role = await _roleManager.FindByNameAsync(roleName);
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

    /// if roles are empty, we return every users
    /// otherwise we return the users have designated roles
    public async IAsyncEnumerable<(User, IList<string>)> ListBasicUserResultAsync(int offset, int limit, IEnumerable<string?>? roles = null)
    {
      if (offset < 0)
      {
        throw new InvalidArgumentException("Offset can't be negative", "offset");
      }
      if (limit < 0)
      {
        throw new InvalidArgumentException("limit can't be negative", "limit"); 
      }
      var sanitized = SanitizedRoles(roles);
      var roleIds = await GetRoleIdsFromNamesAsync(sanitized); 
      
      var query = GetUserQuery(roleIds);
      await foreach (var user in query.OrderBy(u => u.Id).Skip(offset).Take(limit).AsAsyncEnumerable())
      {
        yield return (user, await _userManager.GetRolesAsync(user));
      }
    }

    private async Task<IList<long>> GetRoleIdsFromNamesAsync(IList<string> roles)
    {
      var ids = new List<long>();
      var normalizer = _roleManager.KeyNormalizer;
      await Task.WhenAll(roles.Select(async roleName =>
      {
        var normalized = normalizer.NormalizeName(roleName);
        var role = await _roleManager.FindByNameAsync(normalized);
        if (role == null) {
          throw new InvalidArgumentException($"Unknown role name {roleName}", "roles");
        }
        if (long.TryParse(await _roleManager.GetRoleIdAsync(role), out var roleId)) {
          ids.Add(roleId);
        } else {
          throw new InternalException($"Internal role id mapping unknown. {role}");
        }
      }));
      return ids;
    }

    private IQueryable<User> GetUserQuery(IList<long> roleIds)
    {
      return roleIds.Count == 0 ? _dbContext.Users.AsQueryable() : _dbContext.Users
          .Join(_dbContext.UserRoles.Where("@0.Contains(RoleId)", roleIds), user => user.Id, ur => ur.UserId, (user, userRole) => user).AsQueryable();
    }

    public async Task<int> NumberOfUsersAsync(IEnumerable<string?>? roles = null)
    {
      var sanitized = SanitizedRoles(roles);

      var roleIds = await GetRoleIdsFromNamesAsync(sanitized); 
      var query = GetUserQuery(roleIds);
      return await query.CountAsync();
    }

    public Task<bool> IsAdminAsync(IList<string> roles)
    {
      if (roles == null)
      {
        roles = new List<string>();
      }
      
      var normalizer = _roleManager.KeyNormalizer;
      var adminRoleName = normalizer.NormalizeName("Admin");
      
      return Task.FromResult(roles.Any(r => normalizer.NormalizeName(r) == adminRoleName));
    }
  }

}
