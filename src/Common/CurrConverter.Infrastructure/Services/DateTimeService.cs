using System;
using CurrConverter.Application.Common.Interfaces;

namespace CurrConverter.Infrastructure.Services
{
    public class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}
