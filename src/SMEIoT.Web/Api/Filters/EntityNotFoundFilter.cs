using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SMEIoT.Core.Exceptions;

namespace SMEIoT.Web.Api.Filters
{
  public class EntityNotFoundFilter: IActionFilter, IOrderedFilter
  {
    public int Order { get; } = 0;

    public void OnActionExecuted(ActionExecutedContext context)
    {
      if (context.Exception is EntityNotFoundException exception)
      {
        context.Result = new NotFoundObjectResult(exception.ParamName) {StatusCode = StatusCodes.Status404NotFound};
      }
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
    }
  }
}
