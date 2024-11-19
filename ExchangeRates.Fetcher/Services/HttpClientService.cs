using ExchangeRates.Fetcher.Interfaces;
using ExchangeRates.Shared.Interfaces;
using ExchangeRates.Shared.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeRates.Fetcher.Services
{
    internal class HttpClientService : IHttpClientService
    {
        public HttpClientService(HttpClient httpClient, IExternalApiOptions externalApiOptions)
        {
            _httpClient = httpClient;
            _externalApiUrl = externalApiOptions.Url;
        }

        private readonly HttpClient _httpClient;
        private readonly string _externalApiUrl;

        public async Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync()
        {
            var response = await _httpClient.GetAsync(_externalApiUrl);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to fetch exchange rates: {response.StatusCode}");

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var parsedData = JsonSerializer.Deserialize<ExchangeRateResponse>(jsonResponse);

            if (parsedData is null)
                throw new Exception($"Failed to deserialize response: {jsonResponse}");

            return parsedData.ExchangeRates.Select(rate => new ExchangeRate
            {
                CurrencyCode = rate.Key,
                Rate = rate.Value,
                Date = DateTimeOffset.UtcNow
            });
        }
    }

    internal class ExchangeRateResponse
    {
        [JsonPropertyName("data")]
        public Dictionary<string, double> ExchangeRates { get; set; }
    }
}
