using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using SMEIoT.Web.Api.V1;
using Hangfire;
using NodaTime;
using Microsoft.AspNetCore.Identity;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Interfaces;
using System;
using System.Collections.Generic;

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
      if (user != null)
      {
        BackgroundJob.Enqueue<IUserProfileService>(service => service.UpdateUserLastSeenAsync(user.Id, _clock.GetCurrentInstant().ToDateTimeUtc()));
      }
      await next();
    }
  }
}
