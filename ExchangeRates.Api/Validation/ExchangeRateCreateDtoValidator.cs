using ExchangeRates.Api.Dto;
using FluentValidation;

namespace ExchangeRates.Api.Validation
{
    public class ExchangeRateCreateDtoValidator : AbstractValidator<ExchangeRateCreateDto>
    {
        public ExchangeRateCreateDtoValidator()
        {
            RuleFor(x => x.CurrencyCode)
                .NotEmpty()
                .Length(3)
                .WithMessage("Currency code must be exactly 3 characters.");

            RuleFor(x => x.Rate)
                .GreaterThan(0)
                .WithMessage("Rate must be greater than zero.");

            RuleFor(x => x.Date)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Date cannot be in the future.");
        }
    }
}
