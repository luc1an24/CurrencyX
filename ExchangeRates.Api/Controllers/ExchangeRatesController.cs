using ExchangeRates.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeRates.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ExchangeRatesController : ControllerBase
    {
        public ExchangeRatesController(IExchangeRateService service)
        {
            _service = service;
        }

        private readonly IExchangeRateService _service;

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetExchangeRates([FromQuery] int page = 1, [FromQuery] int pageSize = 100)
        {
            try
            {
                var rates = await _service.GetExchangeRatesAsync(page, pageSize);
                return Ok(rates);
            }
            catch (Exception ex)
            {
                {
                    return StatusCode(500, ex.Message);
                }
            }
        }
        
        [HttpGet("calculate")]
        [Authorize]
        public async Task<IActionResult> GetCalculatedExchangeRate([FromQuery] string currencyCode, [FromQuery] double value)
        {
            if (string.IsNullOrWhiteSpace(currencyCode) || value <= 0)
                return BadRequest("Invalid parameters.");

            try
            {
                var result = await _service.GetCalculatedExchangeRate(currencyCode, value);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("refresh")]
        [Authorize(Policy = "RequireAdmin")]
        public async Task<IActionResult> ForceRefreshData()
        {
            try
            {
                await _service.RefreshExchangeRates();
                return Ok("Exchange rates refreshed successfully.");
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Authorization failed while refreshing data.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
