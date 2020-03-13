using Microsoft.AspNetCore.Mvc;
using ScottLogic.Internal.Training.Matcher;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using Xunit;
using ScottLogic.Internal.Training.Api.Controllers;

namespace ScottLogic.Internal.Training.Api.Tests
{
    public class OrderControllerTests
    {
        [Fact]
        public void Get_ReturnOkStatus_NoExistingOrders()
        {
            var matcherMock = new Mock<IOrderMatcher>();
            matcherMock.SetupGet(matcher => matcher.ExistingOrders).Returns(new List<Order>{});
            var controller = new OrdersController(matcherMock.Object);

            var controllerResponse = controller.Get();
            var objectResponse = controllerResponse as OkObjectResult;

            var expectedOrders = new List<Order> { };

            Assert.Equal(200, objectResponse.StatusCode);
            Assert.Equal(expectedOrders, objectResponse.Value);
        }

        [Fact]
        public void Get_ReturnOkStatus_OneExistingOrder()
        {
            var matcherMock = new Mock<IOrderMatcher>();
            var currentOrder = new Order(1001, 45, 55, OrderType.Sell, 14);
            matcherMock.SetupGet(matcher => matcher.ExistingOrders).Returns(new List<Order> { currentOrder });
            
            var controller = new OrdersController(matcherMock.Object);

            var controllerResponse = controller.Get();
            var objectResponse = controllerResponse as OkObjectResult;

            var expectedOrders = new List<Order> { currentOrder };

            Assert.Equal(200, objectResponse.StatusCode);
            Assert.Equal(expectedOrders, objectResponse.Value);
        }
        
        [Fact]
        public void PostSell_ReturnsOkStatus_Trade()
        {
            var matcherMock = new Mock<IOrderMatcher>();
            var currentOrder = new Order(1001, 45, 55, OrderType.Sell, 14);
            matcherMock.Setup(matcher => matcher.ProcessOrder(currentOrder)).Returns(true);
            var controller = new OrdersController(matcherMock.Object);

            var controllerResponse = controller.Sell(currentOrder);
            var objectResponse = controllerResponse as OkObjectResult;

            Assert.Equal(200, objectResponse.StatusCode);
            Assert.Equal("Match found, Trade created", objectResponse.Value);
        }

        [Fact]
        public void PostSell_ReturnsOkStatus_NoTrade()
        {
            var matcherMock = new Mock<IOrderMatcher>();
            var currentOrder = new Order(1001, 45, 55, OrderType.Sell, 14);
            matcherMock.Setup(matcher => matcher.ProcessOrder(currentOrder)).Returns(false);
            var controller = new OrdersController(matcherMock.Object);

            var controllerResponse = controller.Sell(currentOrder);
            var objectResponse = controllerResponse as OkObjectResult;

            Assert.Equal(200, objectResponse.StatusCode);
            Assert.Equal("Match not found, Order added to Existing Orders", objectResponse.Value);
        }

        [Fact]
        public void PostBuy_ReturnsOkStatus_Trade()
        {
            var matcherMock = new Mock<IOrderMatcher>();
            var currentOrder = new Order(1001, 45, 55, OrderType.Buy, 14);
            matcherMock.Setup(matcher => matcher.ProcessOrder(currentOrder)).Returns(true);
            var controller = new OrdersController(matcherMock.Object);

            var controllerResponse = controller.Sell(currentOrder);
            var objectResponse = controllerResponse as OkObjectResult;

            Assert.Equal(200, objectResponse.StatusCode);
            Assert.Equal("Match found, Trade created", objectResponse.Value);
        }

        [Fact]
        public void PostBuy_ReturnsOkStatus_NoTrade()
        {
            var matcherMock = new Mock<IOrderMatcher>();
            var currentOrder = new Order(1001, 45, 55, OrderType.Buy, 14);
            matcherMock.Setup(matcher => matcher.ProcessOrder(currentOrder)).Returns(false);
            var controller = new OrdersController(matcherMock.Object);

            var controllerResponse = controller.Sell(currentOrder);
            var objectResponse = controllerResponse as OkObjectResult;

            Assert.Equal(200, objectResponse.StatusCode);
            Assert.Equal("Match not found, Order added to Existing Orders", objectResponse.Value);
        }
    }
}
