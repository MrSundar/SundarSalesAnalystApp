using Microsoft.AspNetCore.Mvc;
using SalesAnalystApp.Services.Interfaces;
using System.Threading.Tasks;

namespace SalesAnalystApp.Api.Controllers
{
    /// <summary>
    /// Provides endpoints to retrieve sales records.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly ISalesService _service;

        public SalesController(ISalesService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets a filtered and sorted paginated list of sales records.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? country = null,
            [FromQuery] string? segment = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool descending = false)
        {
            var (records, total) = await _service.GetPagedSalesAsync(page, pageSize, country, segment, sortBy, descending);
            return Ok(new { data = records, total });
        }
    }
}
