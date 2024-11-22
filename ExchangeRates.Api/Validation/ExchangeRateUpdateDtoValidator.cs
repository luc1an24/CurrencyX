using ExchangeRates.Api.Dto;
using FluentValidation;

namespace ExchangeRates.Api.Validation
{
    public class ExchangeRateUpdateDtoValidator : AbstractValidator<ExchangeRateUpdateDto>
    {
        public ExchangeRateUpdateDtoValidator()
        {
            RuleFor(x => x.Rate)
                .GreaterThan(0)
                .WithMessage("Rate must be greater than zero.");
        }
    }
}
