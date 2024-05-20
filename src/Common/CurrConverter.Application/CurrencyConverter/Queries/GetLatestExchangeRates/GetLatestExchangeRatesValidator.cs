using CurrConverter.Application.CurrencyConverter.Queries.GetLatestExchangeRates;

using FluentValidation;

namespace CurrConverter.Application.CurrencyConverter.Queries.GetLatestExchangeRate;

public class GetLatestExchangeRatesValidator : AbstractValidator<GetLatestExchangeRatesQuery>
{
    public GetLatestExchangeRatesValidator()
    {
        RuleFor(x => x.BaseCurrency)
                .NotNull()
                .NotEmpty().WithMessage("Base Currency is required.");
    }
}
