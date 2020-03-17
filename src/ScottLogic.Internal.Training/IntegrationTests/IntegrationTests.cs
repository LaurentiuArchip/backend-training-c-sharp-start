using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
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
        public async Task Get_Endpoints_ReturnUnauthorized(string url)
        {
            // Arrange
            var client = _factory.CreateClient();
            var expected = HttpStatusCode.Unauthorized;

            // Act
            var response = await client.GetAsync(url);

            // Assert
            Assert.Equal(expected, response.StatusCode);
        }

        [Theory]
        [InlineData("api/orders")]
        [InlineData("api/trades")]
        public async Task Get_Endpoints_Authorized_ReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var client = _factory.CreateClient();
            var requestLogin = new
            {
                Url = "api/login",
                Body = new
                {
                    Username = "Lau"
                }
            };
            var responseLogin = await client.PostAsync(requestLogin.Url, ContentHelper.GetStringContent(requestLogin.Body));
            var currentTokenString = await responseLogin.Content.ReadAsStringAsync();
            var currentTokenJson = JObject.Parse(currentTokenString);
            var tokenKey = "token";
            var currentToken = currentTokenJson.GetValue(tokenKey).ToString();
            
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", currentToken);

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task PostSellOrder_ReturnUnauthorized()
        {
            // Arrange
            var client = _factory.CreateClient();
            var request = new
            {
                Url = "api/orders/sell",
                Body = new
                {
                    AccountNumber = 1001,
                    Quantity = 50,
                    Price = 60,
                    Action = OrderType.Sell,
                    TimeRank = 17
                }
            };
            var expected = HttpStatusCode.Unauthorized;

            // Act
            var response = await client.PostAsync(request.Url, ContentHelper.GetStringContent(request.Body));
            var value = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task PostSellOrder_Authorized_()
        {
            // Arrange
            var client = _factory.CreateClient();
            var requestLogin = new
            {
                Url = "api/login",
                Body = new
                {
                    Username = "Lau"
                }
            };
            var responseLogin = await client.PostAsync(requestLogin.Url, ContentHelper.GetStringContent(requestLogin.Body));
            var currentTokenString = await responseLogin.Content.ReadAsStringAsync();
            var currentTokenJson = JObject.Parse(currentTokenString);
            string tokenKey = "token";
            var currentToken = currentTokenJson.GetValue(tokenKey).ToString();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", currentToken);
            
            var request = new
            {
                Url = "api/orders/sell",
                Body = new
                {
                    AccountNumber = 1001,
                    Quantity = 50,
                    Price = 60,
                    Action = OrderType.Sell,
                    TimeRank = 17
                }
            };

            // Act
            var response = await client.PostAsync(request.Url, ContentHelper.GetStringContent(request.Body));
            var value = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task PostBuyOrder_ReturnUnauthorized()
        {
            // Arrange
            var client = _factory.CreateClient();
            var request = new
            {
                Url = "api/orders/buy",
                Body = new
                {
                    AccountNumber = 1001,
                    Quantity = 50,
                    Price = 60,
                    Action = OrderType.Buy,
                    TimeRank = 17
                }
            };
            var expected = HttpStatusCode.Unauthorized;

            // Act
            var response = await client.PostAsync(request.Url, ContentHelper.GetStringContent(request.Body));
            var value = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task PostBuyOrder_Authorized_()
        {
            // Arrange
            var client = _factory.CreateClient();
            var requestLogin = new
            {
                Url = "api/login",
                Body = new
                {
                    Username = "Lau"
                }
            };
            var responseLogin = await client.PostAsync(requestLogin.Url, ContentHelper.GetStringContent(requestLogin.Body));
            var currentTokenString = await responseLogin.Content.ReadAsStringAsync();
            var currentTokenJson = JObject.Parse(currentTokenString);
            string tokenKey = "token";
            var currentToken = currentTokenJson.GetValue(tokenKey).ToString();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", currentToken);
            
            var request = new
            {
                Url = "api/orders/buy",
                Body = new
                {
                    AccountNumber = 1001,
                    Quantity = 50,
                    Price = 60,
                    Action = OrderType.Buy,
                    TimeRank = 17
                }
            };

            // Act
            var response = await client.PostAsync(request.Url, ContentHelper.GetStringContent(request.Body));
            var value = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
        }
     }
}
