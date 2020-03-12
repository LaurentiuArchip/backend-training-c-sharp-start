using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileSystemGlobbing;
using ScottLogic.Internal.Training.Matcher;

namespace ScottLogic.Internal.Training.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    
    public class OrdersController : ControllerBase
    {
        private IOrderMatcher _matcher;

        public OrdersController(IOrderMatcher matcher)
        {
            _matcher = matcher;
        }

       // GET api/orders
       [HttpGet]
        public IActionResult Get()
        {
            return Ok(_matcher.ExistingOrders);
        }

        // POST api/orders/buy
        [HttpPost]
        [Route("buy")]
        public IActionResult Buy([FromBody] Order currentOrder)
        {
            //Order currentOrder1 = JsonSerializer.Deserialize<Order>(currentOrder);
            _matcher.ProcessOrder(currentOrder);
            return Ok("processed");
        }

        // POST api/orders/sell
        [HttpPost]
        [Route("sell")]
        public IActionResult Sell([FromBody] Order currentOrder)
        {
            var status = _matcher.ProcessOrder(currentOrder);
           if (status)
           {
               return Ok("Match found, Trade created");
           }
           return Ok("Match not found, Order added to Existing Orders");
        }
    }
}
