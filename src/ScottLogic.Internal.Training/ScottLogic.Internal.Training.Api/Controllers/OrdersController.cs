
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScottLogic.Internal.Training.Matcher;
using System;
using System.Linq;
using System.Security.Claims;

namespace ScottLogic.Internal.Training.Api.Controllers
{
    /// <summary>
    /// Contains all endpoints to access Orders.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private IOrderMatcher _matcher;

        /// <summary>
        /// The class constructor.
        /// </summary>
        /// <param name="matcher"> Instance of OrderMatcher.</param>
        public OrdersController(IOrderMatcher matcher)
        {
            _matcher = matcher;
        }

        // GET api/orders
        /// <summary>
        /// Get the existing orders.
        /// </summary>
        /// <returns>a List with the existing ordres.</returns>
        /// <response code="200">The existing orders are retrieved successfully</response>
        /// <example>GET api/orders</example>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_matcher.ExistingOrders);
        }

        // GET api/orders/{userAccountNumber}
        /// <summary>
        /// Get the existing orders for this particular user AccountNumber.
        /// </summary>
        /// <returns>a List with the existing ordres for a specific account number.</returns>
        /// <response code="200">The private orders book is retrieved successfully</response>
        /// <example>GET api/orders/{1004}</example>
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get(int userAccountNumber)
        {
            var privateOrdersBook = _matcher.ExistingOrders.Where(order => order.AccountNumber == userAccountNumber).ToList();

            return Ok(privateOrdersBook);
        }

        // POST api/orders/buy
        /// <summary>
        /// Places a new order that wants to buy.
        /// </summary>
        /// <param name="currentOrder">The order to add.</param>
        /// <returns>A confirmation message.</returns>
        /// <response code="200">If a Match is found, and a Trade is created</response>
        /// <response code="200">If a Match is not found, and the Order is added to Existing Orders</response>
        /// <response code="400">If the order data posted is invalid</response>
        /// <example>GET api/order/Buy</example>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        [Route("buy")]
        public IActionResult Buy([FromBody] Order currentOrder)
        {
            // Get the user account number from the access token
            var userIdentity = this.User.Identity as ClaimsIdentity;
            var userAccount = 0;
            if (userIdentity != null && userIdentity.HasClaim(c => c.Type == "AccountNumber"))
            {
                userAccount = Int32.Parse(userIdentity.Claims
                    .FirstOrDefault(c => c.Type == "AccountNumber").Value);
            }
            // Check if the order is valid
            if (userAccount == currentOrder.AccountNumber && currentOrder.Action == OrderType.Buy)
            {
                var status = _matcher.ProcessOrder(currentOrder);
                if (status)
                {
                    return Ok("Match found, Trade created");
                }
                return Ok("Match not found, Order added to Existing Orders");
            }
            else
            {
                return BadRequest();
            }
        }

        // POST api/orders/sell
        /// <summary>
        /// Places a new order that wants to sell.
        /// </summary>
        /// <param name="currentOrder">The order to add.</param>
        /// <returns>A confirmation message.</returns>
        /// <response code="200">If a Match is found, and a Trade is created</response>
        /// <response code="200">If a Match is not found, and the Order is added to Existing Orders</response>
        /// <response code="400">If the order data posted is invalid</response>
        /// <example>GET api/order/Sell</example>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        [Route("sell")]
        public IActionResult Sell([FromBody] Order currentOrder)
        {
            // Get the user account number from the access token
            var userIdentity = this.User.Identity as ClaimsIdentity;
            var userAccount = 0;
            if (userIdentity != null && userIdentity.HasClaim(c => c.Type == "AccountNumber"))
            {
                userAccount = Int32.Parse(userIdentity.Claims
                    .FirstOrDefault(c => c.Type == "AccountNumber").Value);
            }
            // Check if the order is valid
            if (userAccount == currentOrder.AccountNumber && currentOrder.Action == OrderType.Sell)
            {
                var status = _matcher.ProcessOrder(currentOrder);
                if (status)
                {
                    return Ok("Match found, Trade created");
                }
                return Ok("Match not found, Order added to Existing Orders");
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
