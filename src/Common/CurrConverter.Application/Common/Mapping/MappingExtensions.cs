using CurrConverter.Application.Common.Models;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrConverter.Application.Common.Mapping;

public static class MappingExtensions
{
    public static Task<PaginatedList<KeyValuePair<TKey, TValue>>> PaginatedListAsync<TKey, TValue>(
    this IDictionary<TKey, TValue> dictionary, int pageNumber, int pageSize)
    {
        var totalCount = dictionary.Count;
        var items = dictionary.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        return Task.FromResult(new PaginatedList<KeyValuePair<TKey, TValue>>(
            items, totalCount, pageNumber, pageSize));
    }
}
