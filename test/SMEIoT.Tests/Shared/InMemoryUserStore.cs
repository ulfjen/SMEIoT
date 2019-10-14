using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SMEIoT.Core.Entities;
using SMEIoT.Models;

namespace SMEIoT.Tests.Shared
{
  public class InMemoryUserStore: IUserStore<User>, IUserPasswordStore<User>, IUserSecurityStampStore<User>, IUserRoleStore<User>
  {
    private List<User> _users = new List<User>();
    private Dictionary<string, List<string>> _roles = new Dictionary<string, List<string>>();

    public Task AddClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
    {
      if (!_roles.ContainsKey(user.NormalizedUserName))
      {
        _roles[user.NormalizedUserName] = new List<string>();
      }

      if (_roles[user.NormalizedUserName].Any(r => r == roleName))
      {
        return Task.FromResult(IdentityResult.Failed(
          new IdentityError {Description = "A role like that already exists"}
        ));
      }

      _roles[user.NormalizedUserName].Add(roleName);
            return Task.FromResult(IdentityResult.Success);

    }

    public Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
    {
      if (_users.Any(u => (u.Id == user.Id) || (u.NormalizedUserName == user.NormalizedUserName)))
      {
        return Task.FromResult(IdentityResult.Failed(
          new IdentityError {Description = "A user like that already exists"}
        ));
      }

      user.Id = _users.Count + 1;
      _users.Add(user);
      return Task.FromResult(IdentityResult.Success);

    }

    public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
    {
      if (_users.All(u => u.Id != user.Id))
      {
        return Task.FromResult(IdentityResult.Failed(
          new IdentityError {Description = "User does not exist"}
        ));
      }

      _users.Remove(user);
      return Task.FromResult(IdentityResult.Success);
    }

    public void Dispose()
    {
    }

    public Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
      var user = _users.SingleOrDefault(u => u.Id.ToString() == userId);
      return Task.FromResult(user);
    }

    public Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
      var user = _users.SingleOrDefault(u => u.NormalizedUserName == normalizedUserName);
      return Task.FromResult(user);
    }

    public Task<IList<Claim>> GetClaimsAsync(User user, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
    {
      return Task.FromResult(user.UserName.ToUpper());
    }

    public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
    {
      return Task.FromResult(user.PasswordHash);
    }

    public Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
    {
      IList<string> roles = !_roles.ContainsKey(user.NormalizedUserName) ? new List<string>() : _roles[user.NormalizedUserName];
      return Task.FromResult(roles);
    }

    public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
    {
      return Task.FromResult(user.Id.ToString());
    }

    public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
    {
      return Task.FromResult(user.UserName);
    }

    public Task<IList<User>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
    {
      if (!_roles.ContainsKey(user.NormalizedUserName))
      {
        _roles[user.NormalizedUserName] = new List<string>();
      }
      return Task.FromResult(_roles[user.NormalizedUserName].SingleOrDefault(r => r == roleName) != null);
    }

    public Task RemoveClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task ReplaceClaimAsync(User user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
    {
      user.NormalizedUserName = normalizedName;
      return Task.CompletedTask;
    }

    public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
    {
      user.PasswordHash = passwordHash;
      return Task.CompletedTask;
    }

    public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
    {
      user.UserName = userName;
      return Task.CompletedTask;
    }

    public Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
    {
      return Task.FromResult(IdentityResult.Success);
    }

    public Task<string> GetSecurityStampAsync(User user, CancellationToken cancellationToken)
    {
      return Task.FromResult(user.SecurityStamp);
    }

    public Task SetSecurityStampAsync(User user, string stamp, CancellationToken cancellationToken)
    {
      user.SecurityStamp = stamp;
      return Task.CompletedTask;
    }
  }
}
