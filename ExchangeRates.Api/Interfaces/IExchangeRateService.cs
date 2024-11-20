using ExchangeRates.Api.Dto;

namespace ExchangeRates.Api.Interfaces
{
    public interface IExchangeRateService
    {
        Task<IEnumerable<ExchangeRateDto>> GetExchangeRatesAsync(int page, int pageSize);
        Task<double> GetCalculatedExchangeRate(string currencyCode, double eurValue);
        Task RefreshExchangeRates();
    }
}
