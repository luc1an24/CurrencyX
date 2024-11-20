namespace ExchangeRates.Fetcher.Interfaces
{
    public interface IFetcherService
    {
        Task FetchAndSaveExchangeRatesAsync(int failCounter);
    }
}
