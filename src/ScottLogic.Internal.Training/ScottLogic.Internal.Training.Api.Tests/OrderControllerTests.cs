using Microsoft.AspNetCore.Mvc;
using ScottLogic.Internal.Training.Matcher;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;

namespace ScottLogic.Internal.Training.Api.Tests
{
    public class OrderControllerTests
    {
        [Fact]
        public async Task GetOrders_ReturnOKResult()
        {
            var apiClient = new HttpClient();
            var apiResponse =await apiClient.GetAsync("https://localhost:44342/api/orders");

            Assert.True(apiResponse.IsSuccessStatusCode);
        }

        [Fact]
        public async Task PostSellOrder_ReturnOKResult()
        {
            var apiClient = new HttpClient();
            var currentOrder = new Order(1001, 65, 55, OrderType.Sell, 14);
            var data = createHttpContent(currentOrder);
            var apiResponse = await apiClient.PostAsync("https://localhost:44342/api/orders/sell", data);

            Assert.True(apiResponse.IsSuccessStatusCode);
        }

        [Fact]
        public async Task PostBuyOrder_ReturnOKResult()
        {
            var apiClient = new HttpClient();
            var currentOrder = new Order(1001, 65, 55, OrderType.Buy, 14);
            var data = createHttpContent(currentOrder);
            var apiResponse = await apiClient.PostAsync("https://localhost:44342/api/orders/buy", data);

            Assert.True(apiResponse.IsSuccessStatusCode);
        }

        private static HttpContent createHttpContent(Order order)
        {
            HttpContent httpContent = null;
            if (order != null)
            {
                var ms = new MemoryStream();
                SerializeJsonIntoStream(order, ms);
                ms.Seek(0, SeekOrigin.Begin);
                httpContent = new StreamContent(ms);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }
            return httpContent;
        }

        public static void SerializeJsonIntoStream(object value, Stream stream)
        {
            using (var sw = new StreamWriter(stream, new UTF8Encoding(false), 1024, true))
            using (var jtw = new JsonTextWriter(sw) { Formatting = Formatting.None })
            {
                var js = new JsonSerializer();
                js.Serialize(jtw, value);
                jtw.Flush();
            }
        }
    }
}
