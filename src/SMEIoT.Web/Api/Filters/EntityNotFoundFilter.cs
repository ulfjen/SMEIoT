using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SMEIoT.Core.Exceptions;

namespace SMEIoT.Web.Api.Filters
{
  public class EntityNotFoundFilter: IAsyncExceptionFilter, IOrderedFilter
  {
    public int Order { get; } = 1;

    public Task OnExceptionAsync(ExceptionContext context)
    {
      if (context.Exception is EntityNotFoundException exception)
      {
        context.Result = new NotFoundObjectResult(exception.ParamName) {StatusCode = StatusCodes.Status404NotFound};
      }
      return Task.CompletedTask;
    }
  }
}
