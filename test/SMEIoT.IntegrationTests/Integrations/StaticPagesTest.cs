﻿using System.Threading.Tasks;
using SMEIoT.Web;
using Xunit;

namespace SMEIoT.IntegrationTests.Integrations
{
  public class StaticPagesTest: IClassFixture<SeedWebApplicationFactory<Startup>>
  {
    private readonly SeedWebApplicationFactory<Startup> _factory;

    public StaticPagesTest(SeedWebApplicationFactory<Startup> factory)
    {
      _factory = factory;
    }

    [Theory]
    [InlineData("/Privacy")]
    public async Task Show_EndpointsReturnSuccessAndCorrectContentType(string url)
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
