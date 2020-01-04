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
    Task<bool> CreateUserWithPassword(string userName, string password);
    Task<bool> UpdateUserPassword(string userName, string currentPassword, string newPassword);
    Task<bool> UpdateUserRoles(string userName, IEnumerable<string> roles);
    Task<bool> IsAdmin(IEnumerable<string> roles);
    IAsyncEnumerable<(User, IList<string>)> ListBasicUserResultAsync(int start, int limit);
  }
}
