namespace GeniusSquare.WebAPI.Caching;

public interface IAsyncCachedEnumerable<T> : IAsyncEnumerable<T>, IAsyncDisposable
{
    ValueTask<int> GetCacheCountAsync(CancellationToken cancellationToken = default);
}
