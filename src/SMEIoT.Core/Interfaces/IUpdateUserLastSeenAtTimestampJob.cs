using System;
using System.Threading.Tasks;
using SMEIoT.Core.Jobs;

namespace SMEIoT.Core.Interfaces
{
  public interface IUpdateUserLastSeenAtTimestampJob
  {
    void Update(long userId, DateTime seenAt);

    [ThrottleFilter(120, "{0}")]
    Task UpdateAsync(long userId, DateTime seenAt);
  }
}
