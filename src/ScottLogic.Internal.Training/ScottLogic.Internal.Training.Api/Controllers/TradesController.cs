using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScottLogic.Internal.Training.Matcher;

namespace ScottLogic.Internal.Training.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TradesController : ControllerBase
    {
        private readonly IOrderMatcher _matcher;

        public TradesController(IOrderMatcher matcher)
        {
            _matcher = matcher;
        }

        // GET: api/Trades
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_matcher.ExistingTrades);
        }
    }
}
