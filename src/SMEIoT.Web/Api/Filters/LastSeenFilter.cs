using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Hangfire;
using NodaTime;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Interfaces;

namespace SMEIoT.Web.Api.Filters
{
  public class LastSeenFilter : IAsyncActionFilter, IOrderedFilter
  {
    private readonly IClock _clock;
    private readonly UserManager<User> _userManager;
    private readonly ILogger _logger;
    private readonly IApplicationDbContext _dbContext;

    public int Order { get; } = 0;

    public LastSeenFilter(IClock clock, UserManager<User> userManager, ILogger<LastSeenFilter> logger, IApplicationDbContext dbContext)
    {
      _clock = clock;
      _userManager = userManager;
      _logger = logger;
      _dbContext = dbContext;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
      var userClaimed = await _userManager.GetUserAsync(context.HttpContext.User);
      if (userClaimed != null) {
        var user = await _dbContext.Users.FindAsync(userClaimed.Id);
        if (user != null)
        {
          var now = _clock.GetCurrentInstant();
          _logger.LogDebug($"checking last_seen_at timestamp {user.LastSeenAt} (now {now}) for user {user.Id}");
          if (user.LastSeenAt + Duration.FromMinutes(2) < now)
          {
            BackgroundJob.Enqueue<IUpdateUserLastSeenAtTimestampJob>(service => service.UpdateAsync(user.Id, now.ToDateTimeUtc()));
          }
        }
      }
      await next();
    }
  }
}
