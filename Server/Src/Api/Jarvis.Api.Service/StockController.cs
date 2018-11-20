using System.Threading.Tasks;
using Jarvis.Infrastructure.Logging;
using Microsoft.AspNetCore.Mvc;

namespace Jarvis.Api.Service
{
    [Route("stock")]
    public class StockController : ControllerBase
    {
        private readonly ILog _log;

        public StockController(ILog log)
        {
            _log = log;
        }

        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetAsync(string customerId)
        {
            _log.LogInfo("GetAsync called with customer id {CustomerId}", customerId);
            return Ok();
        }
    }
}
