using System.Collections.Generic;
using System.Threading.Tasks;
using SMEIoT.Core.Entities;
using System.Security.Claims;

namespace SMEIoT.Core.Interfaces
{
  public interface IUserManagementService
  {
    Task<(User, IList<string>)> GetUserAndRoleByName(string userName);
    Task<(User, IList<string>)> GetUserAndRoleByPrincipal(ClaimsPrincipal principal);
    Task CreateUserWithPassword(string userName, string password);
    Task UpdateUserPassword(string userName, string currentPassword, string newPassword);
    Task UpdateUserRoles(string userName, IEnumerable<string> roles);
    Task<bool> IsAdmin(IEnumerable<string> roles);
    IAsyncEnumerable<(User, IList<string>)> ListBasicUserResultAsync(int offset, int limit, IEnumerable<string>? roles);
    Task<int> NumberOfUsersAsync(IEnumerable<string>? roles);
  }
}
