using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Exceptions;
using SMEIoT.Core.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace SMEIoT.Web.Services
{
  public class UserCookie
  {
    public string UserName { get; set; }
    public bool Admin { get; set; }

    public UserCookie(string userName, bool admin = false)
    {
      UserName = userName;
      Admin = admin;
    }
  }

  public static class UserCookieManagementService
  {
    private static string UserCookieKey = "currentUser";

    /// <summary>
    /// Serialized some attributes about the current user for our web app to work
    /// </summary>
    public static async Task UserSignedInAsync(CookieSignedInContext context)
    {
      if (context.HttpContext.RequestServices == null)
      {
        throw new InvalidOperationException("RequestServices is null.");
      }

      var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<User>>();
      var service = context.HttpContext.RequestServices.GetRequiredService<IUserManagementService>();

      var user = await userManager.GetUserAsync(context.Principal);
      if (user == null) {
        throw new InvalidOperationException("Cannot get user from cookie context.");
      }
      var roles = await userManager.GetRolesAsync(user);

      var isAdmin = await service.IsAdminAsync(roles);

      var userCookie = new UserCookie(user.UserName, isAdmin);
      var serializeOptions = new JsonSerializerOptions
      {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
      };
      var builtCookie = JsonSerializer.Serialize<UserCookie>(userCookie, serializeOptions);
      
      var builder = new CookieBuilder
      {
        HttpOnly = false,
        IsEssential = true,
        Expiration = context.Options.ExpireTimeSpan
      };
      var cookie = builder.Build(context.HttpContext);
      context.Options.CookieManager.AppendResponseCookie(
          context.HttpContext,
          UserCookieKey,
          builtCookie,
          cookie);
    }

    public static Task UserSigningOutAsync(CookieSigningOutContext context)
    {
      context.Options.CookieManager.DeleteCookie(
          context.HttpContext,
          UserCookieKey,
          context.CookieOptions);
      return Task.CompletedTask;
    }
  }
}
