using System.Threading.Tasks;
using SMEIoT.Web;
using Xunit;

namespace SMEIoT.IntegrationTests.Integrations
{
  public class StaticPagesTest: IClassFixture<SeedWebApplicationFactory<TestStartup>>
  {
    private readonly SeedWebApplicationFactory<TestStartup> _factory;

    public StaticPagesTest(SeedWebApplicationFactory<TestStartup> factory)
    {
      _factory = factory;
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/dashboard")]
    public async Task Show_EndpointsReturnSuccessAndCorrectContentType(string url)
    {
      // Arrange
      var client = _factory.CreateClient();

      // Act
      var response = await client.GetAsync(url);

      // Assert
      response.EnsureSuccessStatusCode();
      Assert.Equal("text/html", response.Content.Headers.ContentType.ToString());
    }
    
 
  }
}
