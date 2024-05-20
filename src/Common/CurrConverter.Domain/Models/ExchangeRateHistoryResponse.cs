using CurrConverter.Domain.Common;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;

namespace CurrConverter.Domain.Models;

public class ExchangeRateHistoryResponse
{
    [JsonProperty("amount")]
    public double Amount { get; set; }

    [JsonProperty("base")]
    public string BaseCurrency { get; set; }

    [JsonProperty("start_date")]
    [JsonConverter(typeof(DateOnlyConverter))]
    public DateOnly StartDate { get; set; }

    [JsonProperty("end_date")]
    [JsonConverter(typeof(DateOnlyConverter))]
    public DateOnly EndDate { get; set; }

    [JsonProperty("rates")]
    [JsonConverter(typeof(RatesConverter))]
    public Dictionary<DateOnly, Dictionary<string, double>> Rates { get; set; }
}
