
namespace GeniusSquare.WebAPI.Caching;

public interface IAsyncCache<TKey, TValue> where TKey : notnull
{
    ValueTask<TValue> GetOrAddAsync(TKey key, Func<TValue> valueFactory, CancellationToken cancellationToken = default);
}