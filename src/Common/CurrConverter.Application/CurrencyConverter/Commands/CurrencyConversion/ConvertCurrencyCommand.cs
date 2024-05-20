using CurrConverter.Application.Common.Interfaces;
using CurrConverter.Application.Common.Models;

using System.Threading.Tasks;
using System.Threading;

using MapsterMapper;
using CurrConverter.Application.Dto;

namespace CurrConverter.Application.CurrencyConverter.Commands.CurrencyConvert;

public class ConvertCurrencyCommand : IRequestWrapper<ConvertCurrencyDto>
{
    public double Amount { get; set; }
    public string FromCurrency { get; set; }
    public string ToCurrency { get; set; }
}

public class ConvertCurrencyHandler : IRequestHandlerWrapper<ConvertCurrencyCommand, ConvertCurrencyDto>
{
    private readonly IMapper _mapper;
    private readonly ICurrencyConverterService _currencyConverterService;

    public ConvertCurrencyHandler(IMapper mapper, ICurrencyConverterService currencyConverterService)
    {
        _mapper = mapper;
        _currencyConverterService = currencyConverterService;
    }

    public async Task<ServiceResult<ConvertCurrencyDto>> Handle(ConvertCurrencyCommand command, CancellationToken cancellationToken)
    {
        var convertCurrencyRresponse = await _currencyConverterService.ConvertCurrencyAsync(command, cancellationToken).ConfigureAwait(false);

        return convertCurrencyRresponse == default
            ? ServiceResult.Failed<ConvertCurrencyDto>(ServiceError.NotFound)
            : ServiceResult.Success(_mapper.Map<ConvertCurrencyDto>(convertCurrencyRresponse));
    }
}
