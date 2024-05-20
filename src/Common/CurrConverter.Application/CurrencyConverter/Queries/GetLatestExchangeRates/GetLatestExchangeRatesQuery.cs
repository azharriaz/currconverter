using CurrConverter.Application.Common.Interfaces;
using CurrConverter.Application.Common.Models;
using CurrConverter.Application.Dto;

using MapsterMapper;

using System.Threading.Tasks;
using System.Threading;

namespace CurrConverter.Application.CurrencyConverter.Queries.GetLatestExchangeRates;

public class GetLatestExchangeRatesQuery : IRequestWrapper<ExchangeRatesDto>
{
    public string BaseCurrency { get; set; }
}

public class RetrieveLatestExchangeRatesHandler : IRequestHandlerWrapper<GetLatestExchangeRatesQuery, ExchangeRatesDto>
{
    private readonly IMapper _mapper;
    private readonly ICurrencyConverterService _currencyConverterService;

    public RetrieveLatestExchangeRatesHandler(IMapper mapper, ICurrencyConverterService currencyConverterService)
    {
        _mapper = mapper;
        _currencyConverterService = currencyConverterService;
    }

    public async Task<ServiceResult<ExchangeRatesDto>> Handle(GetLatestExchangeRatesQuery query, CancellationToken cancellationToken)
    {
        var exchangeRates = await _currencyConverterService.GetExchangeRatesAsync(query.BaseCurrency, cancellationToken).ConfigureAwait(false);

        return exchangeRates == default 
            ? ServiceResult.Failed<ExchangeRatesDto>(ServiceError.NotFound) 
            : ServiceResult.Success(_mapper.Map<ExchangeRatesDto>(exchangeRates));
    }
}
