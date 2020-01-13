using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SMEIoT.Core.Entities;

namespace SMEIoT.Tests.Shared
{
  public class InMemoryUserStore: IUserStore<User>, IUserPasswordStore<User>, IUserSecurityStampStore<User>, IUserRoleStore<User>
  {
    private readonly List<User> _users = new List<User>();
    private readonly Dictionary<long, List<string>> _roles = new Dictionary<long, List<string>>();
    private readonly InMemoryRoleStore _roleStore;

    public InMemoryUserStore(InMemoryRoleStore roleStore)
    {
      _roleStore = roleStore;
    }
    
    public Task AddClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public async Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
    {
      var name = user.NormalizedUserName ?? user.UserName.ToUpperInvariant();
      var role = await _roleStore.FindByNameAsync(roleName.ToUpperInvariant(), cancellationToken);
      if (role == null)
      {
        throw new SystemException("Can find IdentityRole. Forgot to add one?");
      }
      if (!_roles.ContainsKey(role.Id))
      {
        _roles[role.Id] = new List<string>();
      }

      if (_roles[role.Id].Any(u => u == user.NormalizedUserName))
      {
        throw new SystemException("A role like that already exists");
      }

      _roles[role.Id].Add(user.NormalizedUserName);
    }

    public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
    {
      var name = user.NormalizedUserName ?? user.UserName.ToUpperInvariant();

      if (await FindByNameAsync(name, cancellationToken) != null)
      {
        return IdentityResult.Failed(
          new IdentityError {Description = "A user like that already exists"}
        );
      }

      user.Id = _users.Count + 1;
      user.NormalizedUserName = name;
      _users.Add(user);
      return IdentityResult.Success;

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

    public async Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
    {
      var name = user.NormalizedUserName ?? user.UserName.ToUpperInvariant();
      var stored = await FindByNameAsync(name, cancellationToken);
      return stored.PasswordHash;
    }

    public async Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
    {
      var roles = new List<string>();
      foreach (var (roleId, names) in _roles)
      {
        if (names.Contains(user.NormalizedUserName))
        {
          var role = await _roleStore.FindByIdAsync(roleId.ToString(), cancellationToken);
          if (role == null)
          {
            throw new SystemException("Can find IdentityRole. Forgot to add one?");
          }
          roles.Add(role.Name);
        }
      }

      return roles;
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

    public async Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
    {
      var role = await _roleStore.FindByNameAsync(roleName.ToUpperInvariant(), cancellationToken);
      if (role == null)
      {
        throw new SystemException("Can find IdentityRole. Forgot to add one?");
      }
      return _roles.Keys.Contains(role.Id) && _roles[role.Id].Contains(user.NormalizedUserName);
    }

    public Task RemoveClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public async Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
    {
      var role = await _roleStore.FindByNameAsync(roleName.ToUpperInvariant(), cancellationToken);
      if (role == null)
      {
        throw new SystemException("Can find IdentityRole. Forgot to add one?");
      }
      if (_roles.Keys.Contains(role.Id))
      {
        _roles[role.Id].Remove(user.NormalizedUserName);
      }
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

    public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
    {
      var name = user.NormalizedUserName ?? user.UserName.ToUpperInvariant();

      var stored = await FindByNameAsync(name, cancellationToken);
      _users.Remove(stored);
      return await CreateAsync(user, cancellationToken);
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
