using Nito.AsyncEx;
using System.Collections.Concurrent;

namespace GeniusSquare.WebAPI.Caching;

public class AsyncCache<TKey, TValue> : IAsyncCache<TKey, TValue> where TKey : notnull
{
    private class CacheItem(TValue value)
    {
        public TValue Value { get; } = value;
        private long _utcTicks;

        public void Touch() => UtcTouched = DateTime.UtcNow;
        public DateTime UtcTouched
        {
            get => new DateTime(Interlocked.Read(ref _utcTicks), DateTimeKind.Utc);
            private set => Interlocked.Exchange(ref _utcTicks, value.Ticks);
        }
    }

    private readonly ConcurrentDictionary<TKey, CacheItem> _cache = new();
    private readonly AsyncLock _mutex = new();

    public async ValueTask<TValue> GetOrAddAsync(
        TKey key,
        Func<TValue> valueFactory,
        CancellationToken cancellationToken = default)
    {
        CacheItem? item;
        if (!_cache.TryGetValue(key, out item))
        {
            using (await _mutex.LockAsync(cancellationToken))
            {
                item = _cache.GetOrAdd(key, _ => new CacheItem(valueFactory()));
            }
        }
        item.Touch();
        return item.Value;
    }
}
