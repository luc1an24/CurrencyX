using System.ComponentModel.DataAnnotations;

namespace ExchangeRates.Api.Dto
{
    public class ExchangeRateCreateDto
    {
        [Required]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency code must be exactly 3 characters.")]
        public required string CurrencyCode { get; set; }

        [Required]
        [Range(0.0001, double.MaxValue, ErrorMessage = "Rate must be higher than 0")]
        public double Rate { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }
}
