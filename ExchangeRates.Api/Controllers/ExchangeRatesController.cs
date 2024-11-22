using ExchangeRates.Api.Dto;
using ExchangeRates.Api.Interfaces;
using ExchangeRates.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeRates.Api.Controllers
{
    [Route("api/exchange-rates")]
    [ApiController]
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
                return StatusCode(500, ex.Message);
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

        [HttpDelete("delete")]
        [Authorize(Policy = "RequireAdmin")]
        public async Task<IActionResult> DeleteExchangeRate(string currencyCode)
        {
            if (currencyCode == null || currencyCode.Length != 3)
                return BadRequest("Invalid parameters.");

            var exchangeRate = await _service.FindExchangeRateByCode(currencyCode);
            if (exchangeRate == null)
            {
                return NotFound();
            }

            try
            {
                await _service.DeleteLatestExchangeRate(currencyCode);
                return Ok($"Exchange rate for {currencyCode} deleted successfully.");
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Authorization failed while deleting data.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Policy = "RequireAdmin")]
        public async Task<IActionResult> CreateExchangeRate([FromBody] ExchangeRateCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _service.CreateExchangeRateAsync(dto);
                return CreatedAtAction(nameof(GetExchangeRates), new { dto.CurrencyCode }, dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{currencyCode}")]
        [Authorize(Policy = "RequireAdmin")]
        public async Task<IActionResult> UpdateExchangeRate(string currencyCode, [FromBody] ExchangeRateUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var updated = await _service.UpdateExchangeRateAsync(currencyCode, dto);
                if (!updated)
                    return NotFound($"Exchange rate with currency code {currencyCode} not found.");
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("search")]
        [Authorize]
        public async Task<IActionResult> SearchExchangeRates([FromQuery] ExchangeRateSearchDto searchParams)
        {
            var results = await _service.SearchExchangeRatesAsync(searchParams.CurrencyCode, searchParams.Date);
            return Ok(results);
        }
    }
}
