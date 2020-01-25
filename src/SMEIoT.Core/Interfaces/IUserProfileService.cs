using System;
using System.Threading.Tasks;
using SMEIoT.Core.Jobs;

namespace SMEIoT.Core.Interfaces
{
  public interface IUserProfileService
  {
    [ThrottleFitler(120, "{0}")]
    Task UpdateUserLastSeenAsync(long userId, DateTime seenAt);
  }
}
