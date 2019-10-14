using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Io;
using AngleSharp.Scripting;

namespace SMEIoT.Tests.Helpers
{
  public class HtmlHelpers
  {
    public static async Task<IHtmlDocument> GetDocumentAsync(HttpResponseMessage response)
    {
      var content = await response.Content.ReadAsStringAsync();

      var config = Configuration.Default;
      var document = await BrowsingContext.New(config)
        .OpenAsync(ResponseFactory, CancellationToken.None);
      return (IHtmlDocument)document;

      void ResponseFactory(VirtualResponse htmlResponse)
      {
        htmlResponse
          .Address(response.RequestMessage.RequestUri)
          .Status(response.StatusCode);

        MapHeaders(response.Headers);
        MapHeaders(response.Content.Headers);

        htmlResponse.Content(content);

        void MapHeaders(HttpHeaders headers)
        {
          foreach (var (key, enumerable) in headers)
          {
            foreach (var value in enumerable)
            {
              htmlResponse.Header(key, value);
            }
          }
        }
      }
    }
  }
}
