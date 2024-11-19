using ExchangeRates.Shared.Models;

namespace ExchangeRates.Fetcher.Interfaces
{
    public interface IHttpClientInterface
    {
        Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync();
    }
}
