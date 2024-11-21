using ExchangeRates.Api.Dto;
using ExchangeRates.Shared.Models;

namespace ExchangeRates.Api.Interfaces
{
    public interface IExchangeRateService
    {
        Task<PagedResponse<ExchangeRateDto>> GetExchangeRatesAsync(int page, int pageSize);
        Task<double> GetCalculatedExchangeRate(string currencyCode, double eurValue);
        Task RefreshExchangeRates();
        Task<ExchangeRate> FindExchangeRateByCode(string currencyCode);
        Task DeleteLatestExchangeRate(string currencyCode);
        Task CreateExchangeRateAsync(ExchangeRateCreateDto exchangeRateCreateDto);
        Task<bool> UpdateExchangeRateAsync(string currencyCode, ExchangeRateUpdateDto dto);
        Task<IEnumerable<ExchangeRate>> SearchExchangeRatesAsync(string currencyCode, DateTime? date);
    }
}
