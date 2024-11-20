using ExchangeRates.Api.Dto;
using ExchangeRates.Api.Interfaces;
using ExchangeRates.Shared.Interfaces;
using ExchangeRates.Shared.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

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

        public async Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync(int page, int pageSize)
        {
            var rates = await _repository.GetExchangeRatesAsync(page, pageSize);

            return rates.Select(rate => new ExchangeRate
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

            return eurValue * rate.Rate;
        }

        public async Task RefreshExchangeRates()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _externalApiOptions.Url);
            request.Headers.Add("apikey", _externalApiOptions.ApiKey);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to fetch exchange rates: {response.StatusCode}");

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var parsedData = JsonSerializer.Deserialize<ExchangeRateResponse>(jsonResponse);

            if (parsedData is null || parsedData.ExchangeRates is null)
                throw new Exception($"Failed to deserialize response: {jsonResponse}");

            var date = DateTimeOffset.UtcNow;
            var rates = parsedData.ExchangeRates.Select(rate => new ExchangeRate
            {
                CurrencyCode = rate.Key,
                Rate = rate.Value,
                Date = date.Date
            });

            foreach (var rate in rates)
            {
                try
                {
                    await _repository.SaveExchangeRateAsync(rate);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while saving: {ex.Message}");
                }
            }
        }

        public async Task<ExchangeRate> FindExchangeRateByCode(string currencyCode)
        {
            if (string.IsNullOrWhiteSpace(currencyCode))
                throw new ArgumentException("Currency code cannot be null or empty.", nameof(currencyCode));

            var rate = await _repository.GetLatestExchangeRateAsync(currencyCode);
            if (rate == null)
                throw new InvalidOperationException($"Exchange rate for currency '{currencyCode}' not found.");

            return rate;
        }

        public async Task DeleteLatestExchangeRate(string currencyCode)
        {
            if (string.IsNullOrWhiteSpace(currencyCode))
                throw new ArgumentException("Currency code cannot be null or empty.", nameof(currencyCode));

            await _repository.DeleteExchangeRateAsync(currencyCode);
        }
    }
}
