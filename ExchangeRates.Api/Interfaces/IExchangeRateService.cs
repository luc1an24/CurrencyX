using ExchangeRates.Api.Dto;
using ExchangeRates.Shared.Models;

namespace ExchangeRates.Api.Interfaces
{
    public interface IExchangeRateService
    {
        Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync(int page, int pageSize);
        Task<double> GetCalculatedExchangeRate(string currencyCode, double eurValue);
        Task RefreshExchangeRates();
        Task<ExchangeRate> FindExchangeRateByCode(string currencyCode);
        Task DeleteLatestExchangeRate(string currencyCode);
    }
}
