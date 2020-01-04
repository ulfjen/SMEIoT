using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SMEIoT.Core.Exceptions;

namespace SMEIoT.Web.Api.Filters
{
  /// a centralized place for outputing our exceptions used throughout services.
  /// It will be added more met(a data in later pipeline by ASP.NET Core. 
  /// see Mvc/Mvc.Core/src/Infrastructure/DefaultProblemDetailsFactory.cs
  public class ActionExceptionFilter: IAsyncExceptionFilter, IOrderedFilter
  {
    public int Order { get; } = 1;
    private ProblemDetailsFactory _pdFactory;

    public ActionExceptionFilter(ProblemDetailsFactory pdFactory)
    {
      _pdFactory = pdFactory;
    }

    public Task OnExceptionAsync(ExceptionContext context)
    {
      if (context.Exception is EntityNotFoundException entityNotFoundException)
      {
        context.Result = new NotFoundObjectResult(entityNotFoundException.ParamName) {StatusCode = StatusCodes.Status404NotFound};
      }
      if (context.Exception is InvalidArgumentException argumentException)
      {
        // we use InvalidArgumentException when our validation fails on particular parameters failed.
        var errors = new ModelStateDictionary();
        errors.AddModelError(argumentException.ParamName, argumentException.Message);
        context.Result = new BadRequestObjectResult(_pdFactory.CreateValidationProblemDetails(context.HttpContext, errors, StatusCodes.Status400BadRequest));
      }
      return Task.CompletedTask;
    }
  }
}
