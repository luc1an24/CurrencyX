using ExchangeRates.Shared.Models;

namespace ExchangeRates.Fetcher.Interfaces
{
    public interface IHttpClientService
    {
        Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync();
    }
}
