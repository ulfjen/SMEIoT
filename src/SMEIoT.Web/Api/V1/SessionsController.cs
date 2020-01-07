using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SMEIoT.Core.Entities;
using SMEIoT.Web.ApiModels;
using SMEIoT.Web.BindingModels;

namespace SMEIoT.Web.Api.V1
{
  public class SessionsController : BaseController
  {
    private readonly SignInManager<User> _signInManager;
    private readonly ILogger _logger;

    public SessionsController(
      SignInManager<User> signInManager,
      ILoggerFactory loggerFactory)
    {
      _logger = loggerFactory.CreateLogger<SessionsController>();
      _signInManager = signInManager;
    }

    [AllowAnonymous]
//    [ValidateAntiForgeryToken]
    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<LoginedApiModel>> Create(LoginBindingModel model)
    {
//      ViewData["ReturnUrl"] = model.ReturnUrl;
//      if (!ModelState.IsValid)
//      {
//        return View(model);
//      }

//      return NotFound();

      var result =
        await _signInManager.PasswordSignInAsync(model.UserName, model.Password, isPersistent: true,
          lockoutOnFailure: false);
      if (!result.Succeeded)
      {
        // FIX: use the right authentication schema
        return Forbid(result.ToString());
      }

      return Ok(new LoginedApiModel("/dashboard"));

#if false
        if (result.RequiresTwoFactor)
        {
          return RedirectToAction(nameof(SendCode), new {ReturnUrl = returnUrl, RememberMe = model.RememberMe});
        }

      if (result.IsLockedOut)
      {
        _logger.LogWarning(2, "User account locked out.");
        return View("Lockout");
      }
      else
      {
        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View(model);
      }
#endif
    }

  }
}
