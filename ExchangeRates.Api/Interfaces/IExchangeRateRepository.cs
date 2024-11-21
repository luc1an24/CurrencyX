using ExchangeRates.Shared.Models;

namespace ExchangeRates.Api.Interfaces
{
    public interface IExchangeRateRepository
    {
        Task<int> GetTotalExchangeRateCountAsync();
        Task<IEnumerable<ExchangeRate>> GetExchangeRatesPagedAsync(int page, int pageSize);
        Task<ExchangeRate?> GetLatestExchangeRateAsync(string currencyCode);
        Task SaveExchangeRateAsync(ExchangeRate exchangeRate);
        Task DeleteExchangeRateAsync(string currencyCode);
        Task UpdateExchangeRateAsync(ExchangeRate exchangeRate);
        Task<IEnumerable<ExchangeRate>> SearchExchangeRatesAsync(string? currencyCode, DateTime? date);
    }
}
