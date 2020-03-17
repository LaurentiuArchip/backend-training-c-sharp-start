using System;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ScottLogic.Internal.Training.Matcher;
using Xunit;
using System.Collections.Generic;
using Newtonsoft.Json;

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

            var expectedOrders = new List<Order>();

            // Act
            var response = await client.GetAsync(url);
            var contentString = await response.Content.ReadAsStringAsync();
            var existingOrders = JsonConvert.DeserializeObject<IList<Order>>(contentString);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
            Assert.Equal(expectedOrders, existingOrders);
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
        public async Task PostOrders_Authorized()
        {
            // Arrange

                // Set up the Http client and the authorization
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
            
                // Post orders
            var currentOrder1 = new Order(1001, 50, 60, OrderType.Sell, 17);
            var request = new
            {
                Url = "api/orders/sell",
                Body = currentOrder1
            };
            await client.PostAsync(request.Url, ContentHelper.GetStringContent(request.Body));

            var currentOrder2 = new Order(1001, 50, 60, OrderType.Sell, 18);
            request = new
            {
                Url = "api/orders/sell",
                Body = currentOrder2
            };
            await client.PostAsync(request.Url, ContentHelper.GetStringContent(request.Body));

            var currentOrder3 = new Order(1001, 50, 60, OrderType.Sell, 1);
            request = new
            {
                Url = "api/orders/sell",
                Body = currentOrder3
            };
            await client.PostAsync(request.Url, ContentHelper.GetStringContent(request.Body));

            var currentOrder4 = new Order(1003, 50, 60, OrderType.Buy, 19);
            request = new
            {
                Url = "api/orders/buy",
                Body = currentOrder4
            };
            await client.PostAsync(request.Url, ContentHelper.GetStringContent(request.Body));

            var expectedOrders = new List<Order>() {currentOrder1, currentOrder2};
            var currentTrade = new Trade(currentOrder4.AccountNumber, currentOrder4.Quantity, currentOrder4.Price, currentOrder4.Action);
            var expectedTrade = new List<Trade>() { currentTrade };

            // Act
            // Get Existing Orders
            var responseGetOrders =await client.GetAsync("/api/orders");
            var existingOrdersString = await responseGetOrders.Content.ReadAsStringAsync();
            var existingOrders = JsonConvert.DeserializeObject<IList<Order>>(existingOrdersString);

            // Get Trade
            var responseGetTrades = await client.GetAsync("/api/trades");
            var existingTradesString = await responseGetTrades.Content.ReadAsStringAsync();
            var existingTrades = JsonConvert.DeserializeObject<IList<Trade>>(existingTradesString);

            // Assert
            responseGetOrders.EnsureSuccessStatusCode();
            Assert.Equal(expectedOrders, existingOrders);
            Assert.Equal(expectedTrade, existingTrades);
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
    }
}
