using System.Collections.Generic;
using System.Threading.Tasks;
using SMEIoT.Core.Entities;

namespace SMEIoT.Core.Interfaces
{
  public interface IUserManagementService
  {
    Task<(User, IList<string>)> GetUserAndRoleByName(string username);
    Task<bool> CreateUserWithPassword(string username, string password);
    Task<bool> UpdateUserPassword(string username, string currentPassword, string newPassword);
    Task<bool> UpdateUserRoles(string username, IEnumerable<string> roles);
    IAsyncEnumerable<(User, IList<string>)> ListBasicUserResultAsync(int start, int limit);
  }
}
