using System;
using System.Threading.Tasks;
using NodaTime;
using SMEIoT.Core.Interfaces;

namespace SMEIoT.Core.Jobs
{
  public class UpdateUserLastSeenAtTimestampJob : IUpdateUserLastSeenAtTimestampJob
  {
    private readonly IApplicationDbContext _dbContext;
    public UpdateUserLastSeenAtTimestampJob(IApplicationDbContext dbContext)
    {
      _dbContext = dbContext;
    }


    public void Update(long userId, DateTime seenAt)
    {
      var user = _dbContext.Users.Find(userId);
      user.LastSeenAt = Instant.FromDateTimeUtc(seenAt);
      _dbContext.Users.Update(user);
      _dbContext.SaveChanges();
    }

    public async Task UpdateAsync(long userId, DateTime seenAt)
    {
      var user = await _dbContext.Users.FindAsync(userId);
      user.LastSeenAt = Instant.FromDateTimeUtc(seenAt);
      _dbContext.Users.Update(user);
      await _dbContext.SaveChangesAsync();
    }
  }
}
