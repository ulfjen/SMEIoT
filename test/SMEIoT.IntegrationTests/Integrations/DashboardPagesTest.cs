using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using SMEIoT.Web;
using Xunit;

namespace SMEIoT.IntegrationTests.Integrations
{
  public class DashboardPagesTest: IClassFixture<SeedWebApplicationFactory<Startup>>
  {
    private readonly SeedWebApplicationFactory<Startup> _factory;

    public DashboardPagesTest(SeedWebApplicationFactory<Startup> factory)
    {
      _factory = factory;
    }
    
    [Fact]
    public async Task Show_DashboardPagesRequireAnAuthenticatedUser()
    {
      // Arrange
      var client = _factory.CreateClient(
        new WebApplicationFactoryClientOptions
        {
          AllowAutoRedirect = false
        });

      // Act
      var response = await client.GetAsync("/dashboard");

      // Assert
      Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
      Assert.StartsWith("http://localhost/login", 
        response.Headers.Location.OriginalString);
    }
  }
}
