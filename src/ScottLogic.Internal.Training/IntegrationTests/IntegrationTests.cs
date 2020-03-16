using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests
{
    public class IntegrationTests
    {
         private HttpClient Client;
        
        [Fact]
        public async Task Test1()
        {
            // Arrange
            var request = "/";

            // Act
            var response = await Client.GetAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
        }
    }
}
