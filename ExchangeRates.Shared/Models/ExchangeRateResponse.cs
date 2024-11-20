using System.Text.Json.Serialization;

namespace ExchangeRates.Shared.Models
{
    public class ExchangeRateResponse
    {
        [JsonPropertyName("data")]
        public Dictionary<string, double>? ExchangeRates { get; set; }
    }
}
