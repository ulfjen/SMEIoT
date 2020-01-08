using System.Linq;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;

namespace SMEIoT.Web.Api.Config
{
  /// <summary>
  /// Represents the Swagger/Swashbuckle operation filter used to document the implicit API version parameter.
  /// </summary>
  public class ApiVersionOperationFilter : IOperationFilter
  {
    /// <summary>
    /// Applies the filter to the specified operation using the given context.
    /// </summary>
    /// <param name="operation">The operation to apply the filter to.</param>
    /// <param name="context">The current operation filter context.</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
      var apiDescription = context.ApiDescription;

      operation.Deprecated |= apiDescription.IsDeprecated();

      if (operation.Parameters == null)
      {
        return;
      }

      foreach (var format in apiDescription.SupportedRequestFormats.Reverse())
      {
        if (operation.RequestBody.Content.ContainsKey(format.MediaType))
        {
          continue;
        }

        var newContent = new Dictionary<string, OpenApiMediaType>();
        foreach (var k in operation.RequestBody.Content.Keys) {
          newContent[format.MediaType] = operation.RequestBody.Content[k];
        }
        operation.RequestBody.Content = newContent;
      }
    }
  }
}
