using Microsoft.AspNetCore.Mvc;

namespace SMEIoT.Web.Controllers
{
  public class ErrorController : Controller
  {
    public IActionResult Error() => Problem();
  }
}
