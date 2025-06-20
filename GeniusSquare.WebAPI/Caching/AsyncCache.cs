using Nito.AsyncEx;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace GeniusSquare.WebAPI.Caching;

public sealed class AsyncCache<TKey, TValue> : IAsyncCache<TKey, TValue> where TKey : notnull
{
    private class CacheItem(TValue value)
    {
        public TValue Value { get; } = value;
        private long _utcTicks = DateTime.MaxValue.Ticks;

        public bool IsNew => _utcTicks == DateTime.MaxValue.Ticks;
        
        public void Touch() => UtcTouched = DateTime.UtcNow;
        public DateTime UtcTouched
        {
            get => new DateTime(Interlocked.Read(ref _utcTicks), DateTimeKind.Utc);
            private set => Interlocked.Exchange(ref _utcTicks, value.Ticks);
        }
    }

    private readonly ConcurrentDictionary<TKey, CacheItem> _cache = new();
    private readonly AsyncLock _mutex = new();

    public int Count => _cache.Count;

    public event EventHandler<CacheItemAddedEventArgs<TKey>>? CacheItemAdded;

    private void OnItemAdded(CacheItemAddedEventArgs<TKey> args) => CacheItemAdded?.Invoke(this, args);

    public async Task<TValue> GetOrAddAsync(
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

            if (item.IsNew)
            {
                OnItemAdded(new(key, _cache.Count));
            }
        }

        item.Touch();
        return item.Value;
    }

    public IAsyncEnumerable<KeyValuePair<TKey, TValue>> RemoveOldestAsync(
        int itemsToRemove,
        CancellationToken cancellationToken = default)
    {
        List<TKey> keysToRemove = _cache
            .OrderBy(kvp => kvp.Value.UtcTouched)
            .Take(itemsToRemove)
            .Select(kvp => kvp.Key)
            .ToList();

        return RemoveAsync(keysToRemove, cancellationToken);
    }

    public IAsyncEnumerable<KeyValuePair<TKey, TValue>> RemoveOlderThanAsync(
        DateTime utcThreshold,
        CancellationToken cancellationToken = default)
    {
        List<TKey> keysToRemove = _cache
            .Where(kvp => kvp.Value.UtcTouched < utcThreshold)
            .OrderBy(kvp => kvp.Value.UtcTouched)
            .Select(kvp => kvp.Key)
            .ToList();

        return RemoveAsync(keysToRemove, cancellationToken);
    }

    private async IAsyncEnumerable<KeyValuePair<TKey, TValue>> RemoveAsync(
        IEnumerable<TKey> keys,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        List<KeyValuePair<TKey, TValue>> removed = new();
        using (await _mutex.LockAsync(cancellationToken))
        {
            foreach (TKey key in keys)
            {
                if (_cache.TryRemove(key, out CacheItem? item))
                {
                    removed.Add(new(key, item.Value));
                }
            }
        }
        foreach(var kvp in removed)
        {
            yield return kvp;
        }
    }
}
