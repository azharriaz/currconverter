using CurrConverter.Application.CurrencyConverter.Commands.CurrencyConvert;
using CurrConverter.Domain.ConfigOptions.CurrencyConverter;

using FluentValidation;

using Microsoft.Extensions.Options;

using System.Linq;

namespace CurrConverter.Application.CurrencyConverter.Commands.CurrencyConversion;

public class ConvertCurrencyValidator : AbstractValidator<ConvertCurrencyCommand>
{
    private readonly CurrencyConverterOptions _currencyConverterOptions;

    public ConvertCurrencyValidator(IOptions<CurrencyConverterOptions> currencyConverterOptions)
    {
        _currencyConverterOptions = currencyConverterOptions.Value;

        RuleFor(x => x.Amount)
            .NotEmpty()
            .WithMessage("Amount is required.")    
            .GreaterThan(0)
            .WithMessage("Amount must be greater than 0.");

        RuleFor(x => x.FromCurrency)
            .NotEmpty()
            .WithMessage("From Currency is required.")
            .Must(NotBeExcludedCurrency)
            .WithMessage("Conversion from the specified currency is not supported.");

        RuleFor(x => x.ToCurrency)
            .NotEmpty()
            .WithMessage("To Currency is required.");
    }

    private bool NotBeExcludedCurrency(string currency)
    {
        return !_currencyConverterOptions.CurrencyExclusions.Contains(currency);
    }
}