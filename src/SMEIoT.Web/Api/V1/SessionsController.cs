using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using SMEIoT.Core.Entities;
using SMEIoT.Web.ApiModels;
using SMEIoT.Web.BindingModels;
using SMEIoT.Core.Exceptions;

namespace SMEIoT.Web.Api.V1
{
  public class SessionsController : BaseController
  {
    private readonly SignInManager<User> _signInManager;
    private readonly ILogger _logger;

    public SessionsController(
      SignInManager<User> signInManager,
      ILogger<SessionsController> logger)
    {
      _logger = logger;
      _signInManager = signInManager;
    }

    [AllowAnonymous]
//    [ValidateAntiForgeryToken]
    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<LoginedApiModel>> Create([BindRequired] LoginBindingModel model)
    {
      var result =
        await _signInManager.PasswordSignInAsync(model.UserName, model.Password, isPersistent: true,
          lockoutOnFailure: false);
      
      if (result.IsLockedOut) {
        throw new InvalidUserInputException("Your account is locked");
      } else if (result.IsNotAllowed) {
        throw new InvalidUserInputException("Your can't log in.");
      } else if (result.RequiresTwoFactor) {
        throw new InternalException("No 2FA should be used.");
      } else if (!result.Succeeded) {
        throw new InvalidUserInputException($"Your username or password is not correct.");
      }

      return Ok(new LoginedApiModel("/dashboard"));
    }

  }
}
