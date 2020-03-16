using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using ScottLogic.Internal.Training.Matcher;
using Xunit;

namespace IntegrationTests
{
    public class IntegrationTests : IClassFixture<WebApplicationFactory<ScottLogic.Internal.Training.Api.Startup>>
    {
        private readonly WebApplicationFactory<ScottLogic.Internal.Training.Api.Startup> _factory;

        public IntegrationTests(WebApplicationFactory<ScottLogic.Internal.Training.Api.Startup> factory)
        {
            _factory = factory;
        }
        
        [Theory]
        [InlineData("api/orders")]
        [InlineData("api/trades")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
    }
}
