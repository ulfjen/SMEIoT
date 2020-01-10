using System;
using System.Threading.Tasks;

namespace SMEIoT.Core.Interfaces
{
  public interface IUserProfileService
  {
    Task UpdateUserLastSeenAsync(long userId, DateTime seenAt);
  }
}
