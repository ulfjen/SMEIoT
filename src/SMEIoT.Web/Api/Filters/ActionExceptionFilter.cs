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
      var errors = new ModelStateDictionary();

      switch (context.Exception) {
        case EntityNotFoundException exception:
          errors.AddModelError(exception.ParamName, exception.Message);
          context.Result = new NotFoundObjectResult(_pdFactory.CreateValidationProblemDetails(context.HttpContext, errors, StatusCodes.Status404NotFound)) {StatusCode = StatusCodes.Status404NotFound};
          break;
        case InvalidArgumentException exception:
          errors.AddModelError(exception.ParamName, exception.Message);
          context.Result = new BadRequestObjectResult(_pdFactory.CreateValidationProblemDetails(context.HttpContext, errors, StatusCodes.Status422UnprocessableEntity));
          break;
        case InvalidUserInputException exception:
          context.Result = new UnprocessableEntityObjectResult(_pdFactory.CreateProblemDetails(context.HttpContext, StatusCodes.Status422UnprocessableEntity, null, null, exception.Message));
          break;
        case InternalException exception:
          context.Result = new ObjectResult(_pdFactory.CreateProblemDetails(context.HttpContext, StatusCodes.Status500InternalServerError, null, null, exception.Message)) { StatusCode = StatusCodes.Status500InternalServerError };
          break;
        default:
          context.Result = new ObjectResult(_pdFactory.CreateProblemDetails(context.HttpContext, StatusCodes.Status500InternalServerError, null, null, "Unhandled exception.")) { StatusCode = StatusCodes.Status500InternalServerError };
          break;
      }
      
      return Task.CompletedTask;
    }
  }
}
