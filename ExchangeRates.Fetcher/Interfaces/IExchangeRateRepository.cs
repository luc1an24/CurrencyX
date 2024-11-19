using ExchangeRates.Shared.Models;

namespace ExchangeRates.Fetcher.Interfaces
{
    public interface IExchangeRateRepository
    {
        Task SaveExchangeRateAsync(ExchangeRate exchangeRate);
    }
}
