using ExchangeRates.Fetcher.Interfaces;

namespace ExchangeRates.Fetcher.Services
{
    internal class FetcherService : IFetcherService
    {
        public FetcherService(IHttpClientService httpClientInterface, IExchangeRateRepository exchangeRateRepository)
        {
            _httpClient = httpClientInterface;
            _exchangeRateRepository = exchangeRateRepository;
        }

        private readonly IHttpClientService _httpClient;
        private readonly IExchangeRateRepository _exchangeRateRepository;

        public async Task FetchAndSaveExchangeRatesAsync()
        {
            var exchangeRates = await _httpClient.GetExchangeRatesAsync();

            foreach (var rate in exchangeRates)
            {
                await _exchangeRateRepository.SaveExchangeRateAsync(rate);
            }
        }
    }
}
