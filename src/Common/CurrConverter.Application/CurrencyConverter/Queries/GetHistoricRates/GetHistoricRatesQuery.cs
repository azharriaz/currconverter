using CurrConverter.Application.Common.Interfaces;
using CurrConverter.Application.Common.Models;
using CurrConverter.Application.Common.Mapping;

using MapsterMapper;

using System;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using CurrConverter.Application.Dto;

namespace CurrConverter.Application.CurrencyConverter.Queries.GetHistoricRates;

public class GetHistoricalRatesQuery : IRequestWrapper<PaginatedGetHistoricRatesDto>
{
    public string BaseCurrency { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public class GetHistoricalRatesHandler : IRequestHandlerWrapper<GetHistoricalRatesQuery, PaginatedGetHistoricRatesDto>
{
    private readonly IMapper _mapper;
    private readonly ICurrencyConverterService _currencyConverterService;

    public GetHistoricalRatesHandler(IMapper mapper, ICurrencyConverterService currencyConverterService)
    {
        _mapper = mapper;
        _currencyConverterService = currencyConverterService;
    }

    public async Task<ServiceResult<PaginatedGetHistoricRatesDto>> Handle(GetHistoricalRatesQuery query, CancellationToken cancellationToken)
    {
        var exchangeRates = await _currencyConverterService.GetHistoricRatesAsync(query, cancellationToken).ConfigureAwait(false);

        if (exchangeRates == default)
        {
            return ServiceResult.Failed<PaginatedGetHistoricRatesDto>(ServiceError.NotFound);
        }

        // Map exchangeRates to GetHistoricRatesDto using Mapster
        var historicRatesDto = _mapper.Map<GetHistoricRatesDto>(exchangeRates);

        // Paginate the Rates dictionary
        var paginatedRates = await exchangeRates.Rates
            .PaginatedListAsync(query.PageNumber, query.PageSize);

        // Create a new instance of GetHistoricRatesDto with paginated Rates
        var paginatedHistoricRatesDto = new GetHistoricRatesDto
        {
            Amount = historicRatesDto.Amount,
            Base = historicRatesDto.Base,
            StartDate = historicRatesDto.StartDate,
            EndDate = historicRatesDto.EndDate,
            Rates = paginatedRates.Items.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value
            )
        };

        // Create a new instance of PaginatedGetHistoricRatesDto
        var paginatedDto = new PaginatedGetHistoricRatesDto
        {
            Data = paginatedHistoricRatesDto,
            PageIndex = paginatedRates.PageIndex,
            TotalPages = paginatedRates.TotalPages,
            TotalCount = paginatedRates.TotalCount
        };

        return ServiceResult.Success(paginatedDto);
    }
}
