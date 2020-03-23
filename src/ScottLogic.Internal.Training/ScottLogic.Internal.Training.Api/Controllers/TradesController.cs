using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScottLogic.Internal.Training.Matcher;

namespace ScottLogic.Internal.Training.Api.Controllers
{
    /// <summary>
    /// Contains all endpoints to access Trades.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TradesController : ControllerBase
    {
        private readonly IOrderMatcher _matcher;

        /// <summary>
        /// The class constructor.
        /// </summary>
        /// <param name="matcher"> Instance of OrderMatcher.</param>
        public TradesController(IOrderMatcher matcher)
        {
            _matcher = matcher;
        }

        // GET: api/Trades
        /// <summary>
        /// Get the existing trades.
        /// </summary>
        /// <returns>a List with the existing trades.</returns>
        /// <response code="200">The existing trades are retrieved successfully</response>
        /// <example>GET api/trades</example>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_matcher.ExistingTrades);
        }
    }
}
