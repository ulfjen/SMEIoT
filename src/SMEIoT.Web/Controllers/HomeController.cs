using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SMEIoT.Core.Entities;

namespace SMEIoT.Web.Controllers
{
  public class HomeController : Controller
  {
    private readonly ILogger<HomeController> _logger;
    private SignInManager<User> _signInManager;

    public HomeController(ILogger<HomeController> logger, SignInManager<User> signInManager)
    {
      _logger = logger;
      _signInManager = signInManager;
    }

    public IActionResult Index()
    {
      if (_signInManager.IsSignedIn(User))
      {
        return RedirectToAction("Index", "Dashboard");
      }
      else
      {
        return View();
      }
    }

    public IActionResult Privacy()
    {
      return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View();
    }
  }
}
