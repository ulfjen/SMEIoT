using System.IO;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using System.Net.Http;
using SMEIoT.IntegrationTests;
using SMEIoT.Web;
using SMEIoT.Web.BindingModels;
using Xunit;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;

namespace SMEIoT.IntegrationsTests.Integrations
{
  public class SessionPagesTest: IClassFixture<SeedWebApplicationFactory<TestStartup>>
  {
    private readonly SeedWebApplicationFactory<TestStartup> _factory;

    public SessionPagesTest(SeedWebApplicationFactory<TestStartup> factory)
    {
      _factory = factory;
    }

#if false
    [Theory]
    [InlineData("/api/sessions")]
    public async Task CreateSession_RedirectsToDashboard(string url)
    {
      // Arrange
      var client = _factory.CreateClient();
      var payload = new LoginBindingModel { 
        UserName = "admin",
        Password = "a-normal-password-123"
      };
      var options = new JsonSerializerOptions {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
      };
      options = options.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
      var ms = new MemoryStream();
      await JsonSerializer.SerializeAsync<LoginBindingModel>(ms, payload, options);
      var httpContent = new StringContent(ms.ToString(), Encoding.UTF8, "application/json");

      // Act
      var response = await client.PostAsync(url, httpContent);

      // Assert
      response.EnsureSuccessStatusCode();
      Assert.Equal("application/json", response.Content.Headers.ContentType.ToString());
    }
  }
#endif
}
