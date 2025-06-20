
namespace GeniusSquare.WebAPI.Caching;

public interface IAsyncCache<TKey, TValue> where TKey : notnull
{
    int Count { get; }

    event EventHandler<CacheItemAddedEventArgs<TKey>>? CacheItemAdded;

    Task<TValue> GetOrAddAsync(TKey key, Func<TValue> valueFactory, CancellationToken cancellationToken = default);

    IAsyncEnumerable<KeyValuePair<TKey, TValue>> RemoveOldestAsync(int itemsToRemove, CancellationToken cancellationToken = default);
    IAsyncEnumerable<KeyValuePair<TKey, TValue>> RemoveOlderThanAsync(DateTime utcThreshold, CancellationToken cancellationToken = default);
}
