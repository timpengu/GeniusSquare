using Nito.AsyncEx;

namespace GeniusSquare.WebAPI.Caching;

internal sealed class AsyncCachedEnumerable<T>(IAsyncEnumerable<T> source) : IAsyncCachedEnumerable<T>
{
    private readonly Cache _cache = new Cache(source.GetAsyncEnumerator());

    public ValueTask DisposeAsync() => _cache.DisposeAsync();
    
    public ValueTask<int> GetCacheCountAsync(CancellationToken cancellationToken = default) => _cache.GetCountAsync(cancellationToken);

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) => new Enumerator(_cache, cancellationToken);

    private sealed class Cache(IAsyncEnumerator<T> generator) : IAsyncDisposable
    {
        private AsyncLock _mutex = new();
        private List<T>? _items = new();
        private IAsyncEnumerator<T>? _generator = generator;

        private List<T> Items => _items ?? throw new ObjectDisposedException(nameof(Cache));

        public async ValueTask DisposeAsync()
        {
            using (await _mutex.LockAsync())
            {
                _items = null!;
                await DisposeGeneratorAsync();
            }
        }

        public async ValueTask<int> GetCountAsync(CancellationToken cancellationToken = default)
        {
            using (await _mutex.LockAsync(cancellationToken))
            {
                return Items.Count();
            }
        }

        public async ValueTask<bool> TryGetIndex(int index, Action<T> action, CancellationToken cancellationToken = default)
        {
            using (await _mutex.LockAsync(cancellationToken))
            {
                bool success = await EnsureCached(index, cancellationToken);
                if (success)
                {
                    action(Items[index]);
                }
                return success;
            }
        }

        private async ValueTask<bool> EnsureCached(int index, CancellationToken cancellationToken)
        {
            while (Items.Count <= index)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!await GenerateNextCachedAsync())
                {
                    return false;
                }
            }
            return true;
        }

        private async ValueTask<bool> GenerateNextCachedAsync()
        {
            if (_generator == null)
            {
                return false;
            }

            if (!await _generator.MoveNextAsync())
            {
                await DisposeGeneratorAsync();
                return false;
            }

            Items.Add(_generator.Current);
            return true;
        }

        private async ValueTask DisposeGeneratorAsync()
        {
            if (_generator != null)
            {
                await _generator.DisposeAsync();
                _generator = null;
            }
        }
    }

    private class Enumerator(Cache cache, CancellationToken cancellationToken) : IAsyncEnumerator<T>
    {
        private readonly Cache _cache = cache;
        private readonly CancellationToken _cancellationToken = cancellationToken;
        private int _index = -1;

        public T Current { get; private set; } = default!;

        public async ValueTask<bool> MoveNextAsync() =>
            await _cache.TryGetIndex(
                ++_index,
                value => Current = value,
                _cancellationToken);

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}