using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;

namespace SMEIoT.Web.Middlewares
{
  public class DevThrottleMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly int _delay;

    public DevThrottleMiddleware(RequestDelegate next, IConfiguration configuration)
    {
      _next = next;
      _delay = configuration.GetSection("SMEIoT")?.GetValue<int>("DevThrottleMiddlewareDelayMillis") ?? 0;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      if (context.Request.Path.StartsWithSegments("/api")) {
        await Task.Delay(_delay);
      }
      
      await _next(context);
    }
  }

}
