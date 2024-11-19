using ExchangeRates.Fetcher.Interfaces;

namespace ExchangeRates.Fetcher.Services
{
    internal class FetcherService : BackgroundService, IFetcherService
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

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested) {
                await FetchAndSaveExchangeRatesAsync();
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}
