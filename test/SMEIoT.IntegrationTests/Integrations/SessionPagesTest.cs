using System.Threading.Tasks;
using SMEIoT.IntegrationTests;
using SMEIoT.Web;
using Xunit;

namespace SMEIoT.IntegrationsTests.Integrations
{
  public class SessionPagesTest: IClassFixture<SeedWebApplicationFactory<Startup>>
  {
    private readonly SeedWebApplicationFactory<Startup> _factory;

    public SessionPagesTest(SeedWebApplicationFactory<Startup> factory)
    {
      _factory = factory;
    }

    [Theory]
    [InlineData("/signup")]
    [InlineData("/login")]
    public async Task Show_GetsPagesWithoutAuthentication(string url)
    {
      // Arrange
      var client = _factory.CreateClient();

      // Act
      var response = await client.GetAsync(url);

      // Assert
      response.EnsureSuccessStatusCode();
      Assert.Equal("text/html; charset=utf-8", 
        response.Content.Headers.ContentType.ToString());
    }
  }
}
