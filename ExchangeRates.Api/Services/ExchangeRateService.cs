using ExchangeRates.Api.Dto;
using ExchangeRates.Api.Interfaces;
using ExchangeRates.Shared.Interfaces;

namespace ExchangeRates.Api.Services
{
    public class ExchangeRateService : IExchangeRateService
    {
        public ExchangeRateService(IExchangeRateRepository repository, IExternalApiOptions externalApiOptions, HttpClient httpClient)
        {
            _repository = repository;
            _externalApiOptions = externalApiOptions;
            _httpClient = httpClient;
        }

        private readonly IExchangeRateRepository _repository;
        private readonly IExternalApiOptions _externalApiOptions;
        private readonly HttpClient _httpClient;

        public async Task<IEnumerable<ExchangeRateDto>> GetExchangeRatesAsync(int page, int pageSize)
        {
            var rates = await _repository.GetExchangeRatesAsync(page, pageSize);

            return rates.Select(rate => new ExchangeRateDto
            {
                CurrencyCode = rate.CurrencyCode,
                Rate = rate.Rate,
                Date = rate.Date
            });
        }

        public async Task<double> GetCalculatedExchangeRate(string currencyCode, double eurValue)
        {
            if (string.IsNullOrWhiteSpace(currencyCode))
                throw new ArgumentException("Currency code cannot be null or empty.", nameof(currencyCode));

            if (eurValue <= 0)
                throw new ArgumentException("Value must be greater than zero.", nameof(eurValue));

            var rate = await _repository.GetLatestExchangeRateAsync(currencyCode);
            if (rate == null)
                throw new InvalidOperationException($"Exchange rate for currency '{currencyCode}' not found.");

            return eurValue * rate.Value;
        }

        public async Task RefreshExchangeRates()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _externalApiOptions.Url);
            request.Headers.Add("apikey", _externalApiOptions.Url);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new UnauthorizedAccessException("Failed to refresh data: Authorization failed.");
            }
        }
    }
}
