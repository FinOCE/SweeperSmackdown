using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SweeperSmackdown.Extensions;

public static class FeedIteratorExtensions
{
    public static async Task<IEnumerable<T>> ReadAllAsync<T>(this FeedIterator<T> feedIterator)
    {
        var results = new List<T>();

        do
        {
            var res = await feedIterator.ReadNextAsync();
            results.AddRange(res);
        }
        while (feedIterator.HasMoreResults);

        return results;
    }
}
