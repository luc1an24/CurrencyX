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

        public async Task FetchAndSaveExchangeRatesAsync(int failCounter)
        {
            try
            {
                var exchangeRates = await _httpClient.GetExchangeRatesAsync();

                foreach (var rate in exchangeRates)
                {
                    try
                    {
                        await _exchangeRateRepository.SaveExchangeRateAsync(rate);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error while saving: {ex.Message}");
                    }
                }
            } 
            catch (Exception ex)
            {
                failCounter++;

                if (failCounter < 3)
                    await FetchAndSaveExchangeRatesAsync(failCounter);

                Console.WriteLine($"Error while fetching: {ex.Message}");
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await FetchAndSaveExchangeRatesAsync(0);
            
            using PeriodicTimer timer = new(TimeSpan.FromDays(1));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await FetchAndSaveExchangeRatesAsync(0);
            }
        }
    }
}
