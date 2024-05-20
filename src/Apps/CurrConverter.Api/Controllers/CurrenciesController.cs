using CurrConverter.Application.Dto;
using CurrConverter.Application.Common.Models;
using CurrConverter.Application.CurrencyConverter.Queries.GetLatestExchangeRates;
using CurrConverter.Application.CurrencyConverter.Commands.CurrencyConvert;
using CurrConverter.Application.CurrencyConverter.Queries.GetHistoricRates;

using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;
using System.Threading;

namespace CurrConverter.Api.Controllers;

/// <summary>
/// Currencies controller
/// </summary>
public class CurrenciesController : BaseApiController
{
    /// <summary>
    /// Retrieves the latest exchange rates for a specified base currency.
    /// </summary>
    /// <param name="query">The query containing the base currency information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Latest exchange rates.</returns>
    /// <response code="200">Returns the latest exchange rates.</response>
    /// <response code="400">If the query is invalid.</response>
    /// <response code="500">If there is an internal server error.</response>
    [HttpGet("latest")]
    [ProducesResponseType(typeof(ServiceResult<ExchangeRatesDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<ServiceResult<ExchangeRatesDto>>> GetLatestAsync([FromQuery] GetLatestExchangeRatesQuery query, CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(query, cancellationToken));
    }

    /// <summary>
    /// Converts an amount from one currency to another.
    /// </summary>
    /// <param name="command">The command containing the amount and currencies to convert.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Converted currency value.</returns>
    /// <response code="200">Returns the converted currency value.</response>
    /// <response code="400">If the command is invalid.</response>
    /// <response code="500">If there is an internal server error.</response>
    [HttpPost("convert")]
    [ProducesResponseType(typeof(ServiceResult<ExchangeRatesDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<ServiceResult<ExchangeRatesDto>>> ConvertCurrencyAsync([FromBody] ConvertCurrencyCommand command, CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(command, cancellationToken));
    }

    /// <summary>
    /// Retrieves historical exchange rates for a specified date range and base currency.
    /// </summary>
    /// <param name="query">The query containing the date range and base currency information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated historical exchange rates.</returns>
    /// <response code="200">Returns the historical exchange rates.</response>
    /// <response code="400">If the query is invalid.</response>
    /// <response code="500">If there is an internal server error.</response>
    [HttpPost("history")]
    [ProducesResponseType(typeof(ServiceResult<PaginatedGetHistoricRatesDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<ServiceResult<PaginatedGetHistoricRatesDto>>> GetLatestAsync([FromBody] GetHistoricalRatesQuery query, CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(query, cancellationToken));
    }
}
