using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SMEIoT.Core.Entities;

namespace SMEIoT.Web.Controllers
{
  [Authorize]
  public class UsersController : Controller
  {
    private readonly UserManager<User> _userManager;
    private readonly ILogger _logger;

    public UsersController(
      UserManager<User> userManager,
      ILoggerFactory loggerFactory)
    {
      _userManager = userManager;
      _logger = loggerFactory.CreateLogger<UsersController>();
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult New(string? returnUrl = null)
    {
      ViewData["ReturnUrl"] = returnUrl;
      return View();
    }

    #region Helpers

    private void AddErrors(IdentityResult result)
    {
      foreach (var error in result.Errors)
      {
        ModelState.AddModelError(string.Empty, error.Description);
      }
    }

    private Task<User> GetCurrentUserAsync()
    {
      return _userManager.GetUserAsync(HttpContext.User);
    }

    #endregion
  }

}
