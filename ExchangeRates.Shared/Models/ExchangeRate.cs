namespace ExchangeRates.Shared.Models
{
    public class ExchangeRate
    {
        public string CurrencyCode { get; set; } = string.Empty;
        public double Rate { get; set; }
        public DateTime Date { get; set; }
    }
}
