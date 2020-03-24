using Microsoft.AspNetCore.Mvc;
using ScottLogic.Internal.Training.Matcher;
using System.Collections.Generic;
using Moq;
using Xunit;
using ScottLogic.Internal.Training.Api.Controllers;
using System.Security.Claims;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace ScottLogic.Internal.Training.Api.Tests
{
    public class ControllerTests
    {
        public class OrdersControllerTests
        {
            [Fact]
            public void Get_ReturnOkStatus_NoExistingOrders()
            {
                var matcherMock = new Mock<IOrderMatcher>();
                matcherMock.SetupGet(matcher => matcher.ExistingOrders).Returns(new List<Order> { });
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
            public void Get_PrivateOrdersBook_UserID_Authorized_ReturnNoOrdersFound()
            {
                var matcherMock = new Mock<IOrderMatcher>();
                var currentOrder1 = new Order(1001, 45, 55, OrderType.Sell, 14);
                var currentOrder2 = new Order(1002, 45, 55, OrderType.Sell, 15);
                var currentOrder3 = new Order(1003, 45, 55, OrderType.Sell, 16);

                var existingOrders = new List<Order> { currentOrder1, currentOrder2, currentOrder3 };
                matcherMock.Setup(matcher => matcher.ExistingOrders).Returns(existingOrders);

                var controller = new OrdersController(matcherMock.Object);
                int userAccountNumber = 1004;

                var controllerResponse = controller.Get(userAccountNumber);
                var objectResponse = controllerResponse as OkObjectResult;

                var expectedOrders = new List<Order> { };

                Assert.Equal(200, objectResponse.StatusCode);
                Assert.Equal(expectedOrders, objectResponse.Value);
            }

            [Fact]
            public void Get_PrivateOrdersBook_UserID_Authorized_ReturnExistingPrivateOrders()
            {
                var matcherMock = new Mock<IOrderMatcher>();
                var currentOrder1 = new Order(1001, 45, 55, OrderType.Sell, 14);
                var currentOrder2 = new Order(1002, 45, 55, OrderType.Sell, 15);
                var currentOrder3 = new Order(1003, 45, 55, OrderType.Sell, 16);

                var existingOrders = new List<Order> { currentOrder1, currentOrder2, currentOrder3 };
                matcherMock.Setup(matcher => matcher.ExistingOrders).Returns(existingOrders);

                var controller = new OrdersController(matcherMock.Object);
                int userAccountNumber = 1001;

                var controllerResponse = controller.Get(userAccountNumber);
                var objectResponse = controllerResponse as OkObjectResult;

                var expectedOrders = new List<Order> { currentOrder1 };

                Assert.Equal(200, objectResponse.StatusCode);
                Assert.Equal(expectedOrders, objectResponse.Value);
            }

            [Fact]
            public void PostSell_ReturnsOkStatus_Trade()
            {
                // Setup a mock matcher
                var matcherMock = new Mock<IOrderMatcher>();
                var currentOrder = new Order(1001, 45, 55, OrderType.Sell, 14);
                matcherMock.Setup(matcher => matcher.ProcessOrder(currentOrder)).Returns(true);

                // Add user Identity data
                var userClaims = new[] { new Claim("AccountNumber", "1001") };
                var userIdentity = new ClaimsIdentity(userClaims);

                // Initialize the controller
                var controller = new OrdersController(matcherMock.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext
                        {
                            User = new ClaimsPrincipal(userIdentity)
                        }
                    }
                };

                // Act
                var controllerResponse = controller.Sell(currentOrder);
                var objectResponse = controllerResponse as OkObjectResult;

                Assert.Equal(200, objectResponse.StatusCode);
                Assert.Equal("Match found, Trade created", objectResponse.Value);
            }

            [Fact]
            public void PostSell_ReturnsOkStatus_NoTrade()
            {
                // Setup a mock matcher
                var matcherMock = new Mock<IOrderMatcher>();
                var currentOrder = new Order(1001, 45, 55, OrderType.Sell, 14);
                matcherMock.Setup(matcher => matcher.ProcessOrder(currentOrder)).Returns(false);

                // Add user Identity data
                var userClaims = new[] { new Claim("AccountNumber", "1001") };
                var userIdentity = new ClaimsIdentity(userClaims);

                // Initialize the controller
                var controller = new OrdersController(matcherMock.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext
                        {
                            User = new ClaimsPrincipal(userIdentity)
                        }
                    }
                };

                // Act
                var controllerResponse = controller.Sell(currentOrder);
                var objectResponse = controllerResponse as OkObjectResult;
               
                Assert.Equal(200, objectResponse.StatusCode);
                Assert.Equal("Match not found, Order added to Existing Orders", objectResponse.Value);
            }

            [Fact]
            public void PostSell_WrongAccountNumber_ReturnsBadRequest()
            {
                // Setup a mock matcher
                var matcherMock = new Mock<IOrderMatcher>();
                var currentOrder = new Order(1001, 45, 55, OrderType.Sell, 14);
                matcherMock.Setup(matcher => matcher.ProcessOrder(currentOrder)).Returns(true);

                // Add user Identity data
                var userClaims = new[] { new Claim("AccountNumber", "1005") };
                var userIdentity = new ClaimsIdentity(userClaims);

                // Initialize the controller
                var controller = new OrdersController(matcherMock.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext
                        {
                            User = new ClaimsPrincipal(userIdentity)
                        }
                    }
                };

                // Act
                var controllerResponse = controller.Sell(currentOrder);
                var objectResponse = controllerResponse as BadRequestResult;

                Assert.Equal(400, objectResponse.StatusCode);
            }

            [Fact]
            public void PostBuy_ReturnsOkStatus_Trade()
            {
                // Setup a mock matcher
                var matcherMock = new Mock<IOrderMatcher>();
                var currentOrder = new Order(1001, 45, 55, OrderType.Buy, 14);
                matcherMock.Setup(matcher => matcher.ProcessOrder(currentOrder)).Returns(true);

                // Add user Identity data
                var userClaims = new[] { new Claim("AccountNumber", "1001") };
                var userIdentity = new ClaimsIdentity(userClaims);

                // Initialize the controller
                var controller = new OrdersController(matcherMock.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext
                        {
                            User = new ClaimsPrincipal(userIdentity)
                        }
                    }
                };

                // Act
                var controllerResponse = controller.Buy(currentOrder);
                var objectResponse = controllerResponse as OkObjectResult;

                Assert.Equal(200, objectResponse.StatusCode);
                Assert.Equal("Match found, Trade created", objectResponse.Value);
            }

            [Fact]
            public void PostBuy_ReturnsOkStatus_NoTrade()
            {
                // Setup a mock matcher
                var matcherMock = new Mock<IOrderMatcher>();
                var currentOrder = new Order(1001, 45, 55, OrderType.Buy, 14);
                matcherMock.Setup(matcher => matcher.ProcessOrder(currentOrder)).Returns(false);
                
                // Add user Identity data
                var userClaims = new[] { new Claim("AccountNumber", "1001") };
                var userIdentity = new ClaimsIdentity(userClaims);

                // Initialize the controller
                var controller = new OrdersController(matcherMock.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext
                        {
                            User = new ClaimsPrincipal(userIdentity)
                        }
                    }
                };

                // Act
                var controllerResponse = controller.Buy(currentOrder);
                var objectResponse = controllerResponse as OkObjectResult;

                Assert.Equal(200, objectResponse.StatusCode);
                Assert.Equal("Match not found, Order added to Existing Orders", objectResponse.Value);
            }

            [Fact]
            public void PostBuy_WrongAccountNumber_ReturnsBadRequest()
            {
                // Setup a mock matcher
                var matcherMock = new Mock<IOrderMatcher>();
                var currentOrder = new Order(1001, 45, 55, OrderType.Buy, 14);
                matcherMock.Setup(matcher => matcher.ProcessOrder(currentOrder)).Returns(true);

                // Add user Identity data
                var userClaims = new[] { new Claim("AccountNumber", "1005") };
                var userIdentity = new ClaimsIdentity(userClaims);

                // Initialize the controller
                var controller = new OrdersController(matcherMock.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext
                        {
                            User = new ClaimsPrincipal(userIdentity)
                        }
                    }
                };

                // Act
                var controllerResponse = controller.Buy(currentOrder);
                var objectResponse = controllerResponse as BadRequestResult;

                Assert.Equal(400, objectResponse.StatusCode);
            }

            [Fact]
            public void PostBuy_InsteadOfSell_ReturnsBadRequest()
            {
                // Setup a mock matcher
                var matcherMock = new Mock<IOrderMatcher>();
                var currentOrder = new Order(1001, 45, 55, OrderType.Buy, 14);
                
                // Add user Identity data
                var userClaims = new[] { new Claim("AccountNumber", "1005") };
                var userIdentity = new ClaimsIdentity(userClaims);

                // Initialize the controller
                var controller = new OrdersController(matcherMock.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext
                        {
                            User = new ClaimsPrincipal(userIdentity)
                        }
                    }
                };

                // Act
                var controllerResponse = controller.Sell(currentOrder);
                var objectResponse = controllerResponse as BadRequestResult;

                Assert.Equal(400, objectResponse.StatusCode);
            }

            [Fact]
            public void PostSell_InsteadOfBuy_ReturnsBadRequest()
            {
                // Setup a mock matcher
                var matcherMock = new Mock<IOrderMatcher>();
                var currentOrder = new Order(1001, 45, 55, OrderType.Sell, 14);
                
                // Add user Identity data
                var userClaims = new[] { new Claim("AccountNumber", "1001") };
                var userIdentity = new ClaimsIdentity(userClaims);

                // Initialize the controller
                var controller = new OrdersController(matcherMock.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext
                        {
                            User = new ClaimsPrincipal(userIdentity)
                        }
                    }
                };

                // Act
                var controllerResponse = controller.Buy(currentOrder);
                var objectResponse = controllerResponse as BadRequestResult;

                Assert.Equal(400, objectResponse.StatusCode);
            }
        }
        public class UserControllerTests
        {
            // TO DO
        }
    }
}
