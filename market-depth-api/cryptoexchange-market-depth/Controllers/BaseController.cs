using CryptoexchangeMarketDepth.Clients.Integrations;
using Microsoft.AspNetCore.Mvc;

namespace CryptoexchangeMarketDepth.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BaseController : ControllerBase
    {

        private readonly ILogger<BaseController> _logger;
        private readonly BitstampApiClient _bitstampClient;

        public BaseController(ILogger<BaseController> logger, BitstampApiClient bitstampClient)
        {
            _logger = logger;
            _bitstampClient = bitstampClient;
        }

        [HttpGet("{marketSymbol}")]
        public async Task<IActionResult> GetOrderBook(string marketSymbol = "btceur")
        {
            try
            {
                var result = await _bitstampClient.GetOrderBookAsync(marketSymbol);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
