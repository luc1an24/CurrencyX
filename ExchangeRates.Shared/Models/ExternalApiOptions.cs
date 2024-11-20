using ExchangeRates.Shared.Interfaces;

namespace ExchangeRates.Shared.Models
{
    public class ExternalApiOptions : IExternalApiOptions
    {
        public required string Url { get; set; }
        public string ApiKey { get; set; } = string.Empty;
    }
}
