using AutoMapper;
using ExchangeRates.Api.Dto;
using ExchangeRates.Api.Interfaces;
using ExchangeRates.Shared.Interfaces;
using ExchangeRates.Shared.Models;
using System.Text.Json;

namespace ExchangeRates.Api.Services
{
    public class ExchangeRateService : IExchangeRateService
    {
        public ExchangeRateService(IExchangeRateRepository repository, ICacheService cacheService, IExternalApiOptions externalApiOptions, HttpClient httpClient, IMapper mapper)
        {
            _repository = repository;
            _externalApiOptions = externalApiOptions;
            _httpClient = httpClient;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        private readonly IExchangeRateRepository _repository;
        private readonly ICacheService _cacheService;
        private readonly IExternalApiOptions _externalApiOptions;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;

        public async Task<PagedResponse<ExchangeRateDto>> GetExchangeRatesAsync(int page, int pageSize)
        {
            string cacheKey = $"exchange_rates_page_{page}_size_{pageSize}";

            var cachedData = await _cacheService.GetAsync<PagedResponse<ExchangeRateDto>>(cacheKey);
            if (cachedData != null)
            {
                return cachedData;
            }

            var totalRecords = await _repository.GetTotalExchangeRateCountAsync();
            var exchangeRates = await _repository.GetExchangeRatesPagedAsync(page, pageSize);

            var exchangeRateDtos = exchangeRates.Select(rate => new ExchangeRateDto
            {
                CurrencyCode = rate.CurrencyCode,
                Rate = rate.Rate,
                Date = rate.Date
            }).ToList();

            var pagedResponse = new PagedResponse<ExchangeRateDto>(exchangeRateDtos, page, pageSize, totalRecords);

            await _cacheService.SetAsync(cacheKey, pagedResponse, TimeSpan.FromDays(1));

            return pagedResponse;
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

            try
            {
                await InvalidateExchangeRateCache();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while invalidating cache: {ex.Message}");
            }

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

            try
            {
                await InvalidateExchangeRateCache();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while invalidating cache: {ex.Message}");
            }
        }

        public async Task CreateExchangeRateAsync(ExchangeRateCreateDto exchangeRateCreateDto)
        {
            var exchangeRate = _mapper.Map<ExchangeRate>(exchangeRateCreateDto);

            await _repository.SaveExchangeRateAsync(exchangeRate);

            try
            {
                await InvalidateExchangeRateCache();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while invalidating cache: {ex.Message}");
            }
        }

        public async Task<bool> UpdateExchangeRateAsync(string currencyCode, ExchangeRateUpdateDto dto)
        {
            var exchangeRate = _mapper.Map<ExchangeRate>(dto);
            exchangeRate.CurrencyCode = currencyCode;

            try
            {
                await _repository.UpdateExchangeRateAsync(exchangeRate);


                try
                {
                    await InvalidateExchangeRateCache();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while invalidating cache: {ex.Message}");
                }

                return true;
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }

        public async Task<IEnumerable<ExchangeRate>> SearchExchangeRatesAsync(string currencyCode, DateTime? date)
        {
            try
            {
                var exchangeRates = await _repository.SearchExchangeRatesAsync(currencyCode, date);

                return exchangeRates;
            }
            catch (Exception ex)
            {
                throw new Exception("Error searching exchange rates.", ex);
            }
        }

        private async Task InvalidateExchangeRateCache()
        {
            var cachePatterns = new[] { "exchange_rates_page_" };

            foreach (var pattern in cachePatterns)
            {
                await _cacheService.RemoveByPatternAsync(pattern);
            }
        }
    }
}
