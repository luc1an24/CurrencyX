namespace ExchangeRates.Api.Dto
{
    public class ExchangeRateDto
    {
        public string CurrencyCode { get; set; } = string.Empty;
        public double Rate { get; set; }
        public DateTime Date { get; set; }
    }
}
