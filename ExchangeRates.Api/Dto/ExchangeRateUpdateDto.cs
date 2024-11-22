using System.ComponentModel.DataAnnotations;

namespace ExchangeRates.Api.Dto
{
    public class ExchangeRateUpdateDto
    {
        [Range(0.0001, double.MaxValue, ErrorMessage = "Rate must be higher than 0")]
        [Required]
        public double Rate { get; set; }
    }
}
