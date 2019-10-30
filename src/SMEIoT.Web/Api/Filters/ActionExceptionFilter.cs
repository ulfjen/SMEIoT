using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SMEIoT.Core.Exceptions;

namespace SMEIoT.Web.Api.Filters
{
  public class ActionExceptionFilter: IAsyncExceptionFilter, IOrderedFilter
  {
    public int Order { get; } = 1;

    public Task OnExceptionAsync(ExceptionContext context)
    {
      if (context.Exception is EntityNotFoundException entityNotFoundException)
      {
        context.Result = new NotFoundObjectResult(entityNotFoundException.ParamName) {StatusCode = StatusCodes.Status404NotFound};
      }
      if (context.Exception is InvalidArgumentException argumentException)
      {
        context.Result = new BadRequestObjectResult(argumentException.ParamName) { StatusCode = StatusCodes.Status400BadRequest };
      }
      return Task.CompletedTask;
    }
  }
}
