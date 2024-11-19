namespace ExchangeRates.Shared.Models
{
    public class ExchangeRate
    {
        public string CurrencyCode { get; set; } = string.Empty;
        public double Rate { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}
