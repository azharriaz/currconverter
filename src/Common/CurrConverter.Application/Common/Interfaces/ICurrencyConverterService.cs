using CurrConverter.Application.CurrencyConverter.Commands.CurrencyConvert;
using CurrConverter.Application.CurrencyConverter.Queries.GetHistoricRates;
using CurrConverter.Domain.Models;

using System.Threading;
using System.Threading.Tasks;

namespace CurrConverter.Application.Common.Interfaces;

public interface ICurrencyConverterService
{
    Task<ExchangeRateResponse> GetExchangeRatesAsync(string baseCurrency, CancellationToken cancellationToken);

    Task<ExchangeRateResponse> ConvertCurrencyAsync(ConvertCurrencyCommand command, CancellationToken cancellationToken);
    
    Task<ExchangeRateHistoryResponse> GetHistoricRatesAsync(GetHistoricalRatesQuery query, CancellationToken cancellationToken);
}
