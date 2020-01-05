using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace SMEIoT.Web.Api.V1
{
  [ApiConventionType(typeof(DefaultApiConventions))]
  [ApiController]
  [Produces("application/json")]
  [Route("api/[controller]")]
  [Consumes(MediaTypeNames.Application.Json)]
  public class BaseController : ControllerBase
  {
  }
}
