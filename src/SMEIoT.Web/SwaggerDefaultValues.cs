using System.Linq;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;

namespace SMEIoT.Web
{
  /// <summary>
  /// Represents the Swagger/Swashbuckle operation filter used to document the implicit API version parameter.
  /// </summary>
  public class SwaggerDefaultValues : IOperationFilter
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
        if (!operation.RequestBody.Content.ContainsKey(format.MediaType))
        {
          continue;
        }

        operation.RequestBody.Content[$"{format.MediaType}; v=1.0"] = operation.RequestBody.Content[format.MediaType];
        operation.RequestBody.Content.Remove(format.MediaType);
      }
    }
  }
}
