using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NodaTime;
using NodaTime.Testing;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Jobs;
using SMEIoT.Infrastructure.Data;
using SMEIoT.Tests.Shared;
using Xunit;

namespace SMEIoT.Tests.Core
{
  [Collection("Database collection")]
#pragma warning disable CA1063 // Implement IDisposable Correctly
  public class UpdateUserLastSeenAtTimestampJobTest : IDisposable
#pragma warning restore CA1063 // Implement IDisposable Correctly
  {
    private readonly ApplicationDbContext _dbContext;
    private readonly FakeClock _clock;
    private Instant _initial;
    private readonly UpdateUserLastSeenAtTimestampJob _job;

    public UpdateUserLastSeenAtTimestampJobTest()
    {
      _initial = SystemClock.Instance.GetCurrentInstant();
      _clock = new FakeClock(_initial);
      _dbContext = ApplicationDbContextHelper.BuildTestDbContext(_clock);
      _job = new UpdateUserLastSeenAtTimestampJob(_dbContext);
    }

#pragma warning disable CA1063 // Implement IDisposable Correctly
    public void Dispose()
#pragma warning restore CA1063 // Implement IDisposable Correctly
    {
      _dbContext.Database.ExecuteSqlInterpolated($"TRUNCATE users CASCADE;");
      _dbContext.Dispose();
    }

    private async Task<User> SeedUserAsync()
    {
      var userName = "erickg";
      var user = new User
      {
        UserName = userName,
        NormalizedUserName = userName.ToUpperInvariant(),
        LastSeenAt = _initial
      };
      _dbContext.Users.Add(user);

      await _dbContext.SaveChangesAsync();
      return user;
    }

    [Fact]
    public async Task UpdateAsync_UpdatesLastSeenAt()
    {
      var user = await SeedUserAsync();
      var future = _initial + Duration.FromMinutes(5);

      await _job.UpdateAsync(user.Id, future.ToDateTimeUtc());

      Assert.Equal(future, user.LastSeenAt);
    }
  }

}
