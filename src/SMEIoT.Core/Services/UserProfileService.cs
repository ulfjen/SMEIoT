using System;
using System.Threading.Tasks;
using NodaTime;
using SMEIoT.Core.Interfaces;

namespace SMEIoT.Core.Services
{
  public class UserProfileService : IUserProfileService
  {
    private readonly IApplicationDbContext _dbContext;

    public UserProfileService(IApplicationDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public async Task UpdateUserLastSeenAsync(long userId, DateTime seenAt)
    {
      var user = await _dbContext.Users.FindAsync(userId);
      user.LastSeenAt = Instant.FromDateTimeUtc(seenAt);
      await _dbContext.SaveChangesAsync();
    }
  }
}
