using CurrConverter.Domain.Models;

using System.Collections.Generic;
using System;

using Mapster;

namespace CurrConverter.Application.Dto;

public class ExchangeRatesDto : IRegister
{
    public double Amount { get; set; }
    public string Base { get; set; }
    public DateOnly Date { get; set; }
    public Dictionary<string, double> Rates { get; set; }

    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ExchangeRateResponse, ExchangeRatesDto>();
    }
}
