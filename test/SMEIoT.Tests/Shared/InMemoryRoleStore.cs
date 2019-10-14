using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SMEIoT.Tests.Shared
{
  class InMemoryRoleStore : IRoleStore<IdentityRole<long>>
  {
    private List<IdentityRole<long>> _roles = new List<IdentityRole<long>>();
    
    public Task<IdentityResult> CreateAsync(IdentityRole<long> role, CancellationToken cancellationToken)
    {
      if (_roles.Any(r => (r.Id == role.Id)))
      {
        return Task.FromResult(IdentityResult.Failed(
          new IdentityError {Description = "A role like that already exists"}
        ));
      }

      _roles.Add(role);
      return Task.FromResult(IdentityResult.Success);
    }

    public Task<IdentityResult> DeleteAsync(IdentityRole<long> role, CancellationToken cancellationToken)
    {
      if (_roles.All(r => r.Id != role.Id))
      {
        return Task.FromResult(IdentityResult.Failed(
          new IdentityError {Description = "Role does not exist"}
        ));
      }

      _roles.Remove(role);
      return Task.FromResult(IdentityResult.Success);
    }

    Task<IdentityRole<long>> IRoleStore<IdentityRole<long>>.FindByIdAsync(string roleId, CancellationToken cancellationToken)
    {
      var role = _roles.SingleOrDefault(r => r.Id.ToString() == roleId);
      return Task.FromResult(role);
    }

    Task<IdentityRole<long>> IRoleStore<IdentityRole<long>>.FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
    {
      var role = _roles.SingleOrDefault(r => r.NormalizedName == normalizedRoleName);
      return Task.FromResult(role);
    }

    public Task<string> GetNormalizedRoleNameAsync(IdentityRole<long> role, CancellationToken cancellationToken)
    {
      return Task.FromResult(role.NormalizedName);
    }

    public Task<string> GetRoleIdAsync(IdentityRole<long> role, CancellationToken cancellationToken)
    {
      return Task.FromResult(role.Id.ToString());
    }

    public Task<string> GetRoleNameAsync(IdentityRole<long> role, CancellationToken cancellationToken)
    {
      return Task.FromResult(role.Name);
    }

    public Task SetNormalizedRoleNameAsync(IdentityRole<long> role, string normalizedName, CancellationToken cancellationToken)
    {
      role.NormalizedName = normalizedName;
      return Task.CompletedTask;
    }

    public Task SetRoleNameAsync(IdentityRole<long> role, string roleName, CancellationToken cancellationToken)
    {
      role.Name = roleName;
      return Task.CompletedTask;
    }

    public Task<IdentityResult> UpdateAsync(IdentityRole<long> role, CancellationToken cancellationToken)
    {
      return Task.FromResult(IdentityResult.Success);
    }

    public void Dispose()
    {
    }
  }
}
