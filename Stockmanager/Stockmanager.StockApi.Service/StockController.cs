
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Stockmanager.Infrastructure.Logging;

namespace Stockmanager.StockApi.Service
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
