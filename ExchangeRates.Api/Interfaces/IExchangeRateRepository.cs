using ExchangeRates.Shared.Models;

namespace ExchangeRates.Api.Interfaces
{
    public interface IExchangeRateRepository
    {
        Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync(int page, int pageSize);
        Task<ExchangeRate?> GetLatestExchangeRateAsync(string currencyCode);
        Task SaveExchangeRateAsync(ExchangeRate exchangeRate);
        Task DeleteExchangeRateAsync(string currencyCode);
    }
}
