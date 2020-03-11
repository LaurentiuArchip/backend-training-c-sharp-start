using System;
using System.Collections.Generic;
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
        private readonly IOrderMatcher _matcher = new OrderMatcher();
       

       // GET api/orders
       [HttpGet]
        public IActionResult Get()
        {
            return Ok(_matcher.ExistingOrders);
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody] Order currentOrder)
        {
            _matcher.ProcessOrder(currentOrder);
            return Ok("processed");
        }

        // POST api/orders
        [HttpPost]
        [Route("buy")]
        public IActionResult Buy([FromBody] Order currentOrder)
        {
            //Order currentOrder1 = JsonSerializer.Deserialize<Order>(currentOrder);
            _matcher.ProcessOrder(currentOrder);
            return Ok("processed");
        }

        // POST api/orders
        [HttpPost]
        [Route("sell")]
        public IActionResult Sell([FromBody] Order currentOrder)
        {
           var status = _matcher.ProcessOrder(currentOrder);
           if (status)
           {
               return Ok(_matcher.CurrentTrade);
           }
           return Ok(status);
        }
    }
}
