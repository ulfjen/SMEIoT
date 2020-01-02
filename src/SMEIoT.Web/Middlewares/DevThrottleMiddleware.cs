using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SMEIoT.Web.Middlewares
{
  public class DevThrottleMiddleware
  {
    private readonly RequestDelegate _next;

    public DevThrottleMiddleware(RequestDelegate next)
    {
      _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      Console.WriteLine(context.Request.Path);
            Console.WriteLine(context.Request.Path.StartsWithSegments("/api"));

      if (context.Request.Path.StartsWithSegments("/api")) {
        await Task.Delay(3000);
      }
      
      await _next(context);
    }
  }

}
