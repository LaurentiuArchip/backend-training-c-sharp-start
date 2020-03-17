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
    public class TradesController : ControllerBase
    {
        private IOrderMatcher _matcher;

        public TradesController(IOrderMatcher matcher)
        {
            _matcher = matcher;
        }

        // GET: api/Trades
        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            return Ok(_matcher.ExistingTrades);
        }
    }
}
