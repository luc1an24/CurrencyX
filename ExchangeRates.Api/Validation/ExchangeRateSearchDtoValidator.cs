using ExchangeRates.Api.Dto;
using FluentValidation;

namespace ExchangeRates.Api.Validation
{
    public class ExchangeRateSearchDtoValidator : AbstractValidator<ExchangeRateSearchDto>
    {
        public ExchangeRateSearchDtoValidator()
        {
            RuleFor(x => x.CurrencyCode)
                .Length(3)
                .When(x => !string.IsNullOrWhiteSpace(x.CurrencyCode))
                .WithMessage("Currency code must be exactly 3 characters.");

            RuleFor(x => x.Date)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Date cannot be in the future.");
        }
    }
}
