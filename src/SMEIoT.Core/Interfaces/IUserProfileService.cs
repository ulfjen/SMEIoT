using System;
using System.Threading.Tasks;

namespace SMEIoT.Core.Interfaces
{
  public interface IUserProfileService
  {
    Task<bool> UpdateUserLastSeenAsync(long userId, DateTime seenAt);
  }
}
