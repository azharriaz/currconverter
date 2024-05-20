using System.Collections.Generic;
using System;

namespace CurrConverter.Domain.Models;

public class ExchangeRateResponse
{
    public double Amount { get; set; }
    public string Base { get; set; }
    public DateOnly Date { get; set; }
    public Dictionary<string, double> Rates { get; set; }
}
