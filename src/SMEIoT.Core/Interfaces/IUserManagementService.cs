using System.Collections.Generic;
using System.Threading.Tasks;
using SMEIoT.Core.Entities;

namespace SMEIoT.Core.Interfaces
{
  public interface IUserManagementService
  {
    Task<(User, IList<string>)> GetUserAndRoleByNameAsync(string userName);
    Task CreateUserWithPasswordAsync(string userName, string password);
    Task UpdateUserPasswordAsync(string userName, string currentPassword, string newPassword);
    Task UpdateUserRolesAsync(string userName, IEnumerable<string?>? roles);
    IAsyncEnumerable<(User, IList<string>)> SearchUserWithQueryAsync(string query, int limit);
    Task<bool> IsAdminAsync(IList<string> roles);
    IAsyncEnumerable<(User, IList<string>)> ListBasicUserResultAsync(int offset, int limit, IEnumerable<string?>? roles);
    Task<int> NumberOfUsersAsync(IEnumerable<string?>? roles);
  }
}
