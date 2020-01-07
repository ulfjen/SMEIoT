using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using SMEIoT.Core.Entities;

namespace SMEIoT.Web.Services
{
  public class SignInManager : SignInManager<User>
  {
    public SignInManager(UserManager<User> userManager,
        IHttpContextAccessor contextAccessor,
        IUserClaimsPrincipalFactory<User> claimsFactory,
        IOptions<IdentityOptions> optionsAccessor,
        ILogger<SignInManager> logger,
        IAuthenticationSchemeProvider schemes,
        IUserConfirmation<User> confirmation)
        : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
    {
    }

    public override async Task SignOutAsync()
    {
      await Context.SignOutAsync(IdentityConstants.ApplicationScheme);
      await Context.SignOutAsync(IdentityConstants.ApplicationScheme);
    }

  }
}
