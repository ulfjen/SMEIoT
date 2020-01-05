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
      switch (context.Exception) {
        case EntityNotFoundException entityNotFoundException:
          context.Result = new NotFoundObjectResult(entityNotFoundException.ParamName) {StatusCode = StatusCodes.Status404NotFound};
          break;
        case InvalidArgumentException argumentException:
          var errors = new ModelStateDictionary();
          errors.AddModelError(argumentException.ParamName, argumentException.Message);
          context.Result = new BadRequestObjectResult(_pdFactory.CreateValidationProblemDetails(context.HttpContext, errors, StatusCodes.Status422UnprocessableEntity));
          break;
        case InvalidUserInputException exception:
          context.Result = new ObjectResult(_pdFactory.CreateProblemDetails(context.HttpContext, StatusCodes.Status422UnprocessableEntity, null, null, exception.Message));
          break;
        case InternalException exception:
          context.Result = new ObjectResult(_pdFactory.CreateProblemDetails(context.HttpContext, StatusCodes.Status500InternalServerError, null, null, exception.Message));
          break;
        default:
          context.Result = new ObjectResult(_pdFactory.CreateProblemDetails(context.HttpContext, StatusCodes.Status500InternalServerError, null, null, "Unhandled exception."));
          break;
      }
      
      return Task.CompletedTask;
    }
  }
}
