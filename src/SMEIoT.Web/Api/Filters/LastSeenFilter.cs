using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Hangfire;
using NodaTime;
using Microsoft.AspNetCore.Identity;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Interfaces;

namespace SMEIoT.Web.Api.Filters
{
  public class LastSeenFilter : IAsyncActionFilter, IOrderedFilter
  {
    private readonly IClock _clock;
    private readonly UserManager<User> _userManager;
    public int Order { get; } = 0;

    public LastSeenFilter(IClock clock, UserManager<User> userManager)
    {
      _clock = clock;
      _userManager = userManager;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
      var user = await _userManager.GetUserAsync(context.HttpContext.User);
      var now = _clock.GetCurrentInstant();
      if (user != null && user.LastSeenAt + Duration.FromMinutes(2) < now)
      {
        BackgroundJob.Enqueue<IUserProfileService>(service => service.UpdateUserLastSeenAsync(user.Id, now.ToDateTimeUtc()));
      }
      await next();
    }
  }
}
