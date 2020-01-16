using System;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using SMEIoT.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace SMEIoT.Web.Services
{
  /// overrides identity behaviour
  public static class SecurityStampValidatorHelper
  {
    /// <summary>
    /// Validates a principal against a user's stored security stamp.
    /// </summary>
    /// <param name="context">The context containing the <see cref="System.Security.Claims.ClaimsPrincipal"/>
    /// and <see cref="AuthenticationProperties"/> to validate.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous validation operation.</returns>
    public static Task ValidatePrincipalAsync(CookieValidatePrincipalContext context)
      => ValidateAsync<SecurityStampValidator<User>>(context);

    /// <summary>
    /// Used to validate the <see cref="IdentityConstants.TwoFactorUserIdScheme"/> and 
    /// <see cref="IdentityConstants.TwoFactorRememberMeScheme"/> cookies against the user's 
    /// stored security stamp.
    /// </summary>
    /// <param name="context">The context containing the <see cref="System.Security.Claims.ClaimsPrincipal"/>
    /// and <see cref="AuthenticationProperties"/> to validate.</param>
    /// <returns></returns>

    public static Task ValidateAsync<TValidator>(CookieValidatePrincipalContext context) where TValidator : ISecurityStampValidator
    {
      if (context.HttpContext.RequestServices == null)
      {
        throw new InvalidOperationException("RequestServices is null.");
      }

      var validator = context.HttpContext.RequestServices.GetRequiredService<TValidator>();
      
      return validator.ValidateAsync(context);
    }
  }
  
  public class StrictSecurityStampValidator : SecurityStampValidator<User>
  {
    public StrictSecurityStampValidator(IOptions<SecurityStampValidatorOptions> options, SignInManager<User> signInManager, ISystemClock clock, ILoggerFactory logger)
      : base(options, signInManager, clock, logger)
    {}

    public override async Task ValidateAsync(CookieValidatePrincipalContext context)
    {
      // validate regardless of explased time
      var user = await VerifySecurityStamp(context.Principal); 
      if (user != null)
      {
        await SecurityStampVerified(user, context);
      }
      else
      {
        Logger.LogDebug(0, "Security stamp validation failed, rejecting cookie.");
        context.RejectPrincipal();
        await SignInManager.SignOutAsync();
      }
    }
  }
}