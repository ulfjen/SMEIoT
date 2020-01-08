using System.Linq;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;

namespace SMEIoT.Web.Api.Config
{
  /*
    Targeting Swashbuckle.AspNetCore 5.0.0-rc4's behaviour.
    Required will not yield undefined fields.
    Nullable will yield null type for fields. 
    BindingModels may have required but nullable value.
    ApiModels should be always required but not nullable unless expressed so.
    If they are not null, [JsonProperty(Required = Required.DisallowNull)] needs
    to be set to indicate this value will not be null for swagger.
    It's because nullable reference types still have reference type as null by default.
    C# compiler guards that situation for us. But it isn't interesting to swashbuckle.
  */
  public class RequiredSchemaFilter : ISchemaFilter
  {
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
      if (schema.Properties == null) {
        return;
      }

      MarkRequiredButNotNullableProperties(schema);

      if (context.ApiModel.Type.Name.Contains("ApiModel")) {
        MarkRequiredApiModelProperties(schema);
      }
    }

    private void MarkRequiredButNotNullableProperties(OpenApiSchema schema)
    {
      var requiredButNullableProperties = schema
        .Properties
        .Where(x => x.Value.Nullable && schema.Required.Contains(x.Key));

      foreach (var property in requiredButNullableProperties) {
        property.Value.Nullable = false;
      }
    }

    // if we enable required on nullable earlier for ApiModels, they would be set to not nullable which we might set so
    private void MarkRequiredApiModelProperties(OpenApiSchema schema)
    {
      foreach (var property in schema.Properties) {
        schema.Required.Add(property.Key);
      }
    }
  }
}
