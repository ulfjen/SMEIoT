using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SMEIoT.Core.Entities;
using SMEIoT.Models.AccountViewModels;

namespace SMEIoT.Web.Controllers
{
  public class SessionsController : Controller
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
    public IActionResult New(string? returnUrl = null)
    {
      ViewData["ReturnUrl"] = returnUrl;
      return View();
    }
//
//    [AllowAnonymous]
//    [ValidateAntiForgeryToken]
//    public async Task<IActionResult> Create(LoginViewModel model)
//    {
//      ViewData["ReturnUrl"] = model.ReturnUrl;
//      if (!ModelState.IsValid)
//      {
//        return View(model);
//      }
//
//      return NotFound();
//
//      var result =
//        await _signInManager.PasswordSignInAsync(model.Username, model.Password, isPersistent: true,
//          lockoutOnFailure: false);
//      if (result.Succeeded)
//      {
//        _logger.LogInformation(1, "User logged in.");
//        return !string.IsNullOrEmpty(model.ReturnUrl) ? RedirectToLocal(model.ReturnUrl) : RedirectToAction("Index", "Dashboard");
//      }
//
//#if false
//        if (result.RequiresTwoFactor)
//        {
//          return RedirectToAction(nameof(SendCode), new {ReturnUrl = returnUrl, RememberMe = model.RememberMe});
//        }
//#endif
//
//      if (result.IsLockedOut)
//      {
//        _logger.LogWarning(2, "User account locked out.");
//        return View("Lockout");
//      }
//      else
//      {
//        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
//        return View(model);
//      }
//    }

    private IActionResult RedirectToLocal(string returnUrl)
    {
      if (Url.IsLocalUrl(returnUrl))
      {
        return Redirect(returnUrl);
      }
      else
      {
        return RedirectToAction(nameof(DashboardController.Index), "Dashboard");
      }
    }
  }
}
