using System.ComponentModel.DataAnnotations;

namespace ExchangeRates.Api.Dto
{
    public class ExchangeRateSearchDto
    {
        [Required]
        public required string CurrencyCode { get; set; }
        public DateTime? Date { get; set; }
    }
}
