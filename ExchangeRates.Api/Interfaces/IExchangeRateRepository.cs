using ExchangeRates.Shared.Models;

namespace ExchangeRates.Api.Interfaces
{
    public interface IExchangeRateRepository
    {
        Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync(int page, int pageSize);
        Task<double?> GetLatestExchangeRateAsync(string currencyCode);
    }
}
