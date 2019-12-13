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

namespace SMEIoT.Core.Services
{
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

    public async Task<(User, IList<string>)> GetUserAndRoleByName(string username)
    {
      var user = await _userManager.FindByNameAsync(username);
      if (user == null)
      {
        throw new EntityNotFoundException($"cannot find the user {username}.", "userName");
      }

      var roles = await _userManager.GetRolesAsync(user);
      return (user, roles);
    }

    public async Task<bool> CreateUserWithPassword(string username, string password)
    {
      var result =
        await _userManager.CreateAsync(new User {UserName = username, SecurityStamp = Guid.NewGuid().ToString()},
          password);
      if (result.Succeeded)
      {
        var storedUser = await _userManager.FindByNameAsync(username);

        // depends on storage provider providing a sequence starting from 1
        if (storedUser.Id <= 1L)
        {
          _logger.LogDebug($"Assign new user {storedUser.UserName} with admin role");
          var roleResult = await _userManager.AddToRoleAsync(storedUser, "Admin");
          if (!roleResult.Succeeded)
          {
            throw new Exception(roleResult.Errors.Select(e => e.Code).ToString());
          }
        }
      }
      else
      {
        throw new InvalidArgumentException("entity", result.ToString());
      }

      return true;
    }

    public async Task<bool> UpdateUserPassword(string username, string currentPassword, string newPassword)
    {
      var user = await _userManager.FindByNameAsync(username);
      if (user == null)
      {
        throw new EntityNotFoundException($"cannot find the user {username}.", "userName");
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

    public async Task<bool> UpdateUserRoles(string username, IEnumerable<string> roles)
    {
      var user = await _userManager.FindByNameAsync(username);
      if (user == null)
      {
        throw new EntityNotFoundException($"cannot find the user {username}.", "userName");
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
  }
}
