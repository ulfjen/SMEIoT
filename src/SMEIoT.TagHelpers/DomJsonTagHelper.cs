using System.Collections.Generic;
using System.Text.Json;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Html;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;

namespace SMEIoT.TagHelpers
{
  public class DomJsonTagHelper : TagHelper
  {
    public Dictionary<string, object>? Objects { get; set; }
    
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
      output.TagName = "script";
      if (Objects == null) { return; }
      var builder = new HtmlContentBuilder();
      var options = new JsonSerializerOptions() {PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
      options.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
      var content = builder.AppendHtml("window.SMEIoTPreRendered=")
        .AppendHtml(JsonSerializer.Serialize(Objects, options))
        .AppendHtml(";");

      output.Content.SetHtmlContent(content);
    }
  }
}
