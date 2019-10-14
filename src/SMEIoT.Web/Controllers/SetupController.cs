using Microsoft.AspNetCore.Mvc;

namespace SMEIoT.Web.Controllers
{
  public class SetupController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }
  }
}
