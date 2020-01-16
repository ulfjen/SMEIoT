using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using SMEIoT.Core.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace SMEIoT.Web.Services
{
  public static class ForbidEventHandler
  {
    /// <summary>
    /// Serialized some attributes about the current user for our web app to work
    /// </summary>
    public static Task OnRedirectToAccessDenied(RedirectContext<CookieAuthenticationOptions> context)
    {
      context.Response.StatusCode = 200;
      return Task.CompletedTask;
    }
  }
}
