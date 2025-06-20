using GeniusSquare.WebAPI.Helpers;

namespace GeniusSquare.WebAPI.Caching;

public class AsyncCacheMonitor<TKey, TValue> : IAsyncDisposable, IAsyncCacheMonitor where TKey : notnull
{
    public static string DefaultCacheName = $"AsyncCache<{typeof(TKey).Name},{typeof(TValue).Name}>";

    private readonly IAsyncCache<TKey, TValue> _cache;
    private readonly ILogger<AsyncCacheMonitor<TKey, TValue>> _logger;

    private readonly Task _monitorTask;
    private readonly CancellationTokenSource _monitorCancellation = new();
    private readonly SemaphoreSlim _overflowSemaphore = new(0);

    public string CacheName { get; }
    public TimeSpan CacheTimeout { get; }
    public int CacheLowWatermark { get; }
    public int CacheHighWatermark { get; }

    public AsyncCacheMonitor(
        IAsyncCache<TKey, TValue> cache,
        ILogger<AsyncCacheMonitor<TKey, TValue>> logger,
        string cacheName,
        TimeSpan cacheTimeout,
        TimeSpan monitorInterval,
        int cacheLowWatermark,
        int cacheHighWatermark,
        int monitorMaxRestarts = 10)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        CacheName = cacheName ?? DefaultCacheName;
        CacheTimeout = cacheTimeout >= TimeSpan.Zero ? cacheTimeout : throw new ArgumentException(nameof(cacheTimeout));
        CacheLowWatermark = cacheLowWatermark >= 0 ? cacheLowWatermark : throw new ArgumentException(nameof(cacheLowWatermark));
        CacheHighWatermark = cacheHighWatermark >= cacheLowWatermark ? cacheHighWatermark : throw new ArgumentException(nameof(cacheHighWatermark));

        _monitorTask = MonitorWithRestarts(monitorMaxRestarts, monitorInterval, _monitorCancellation.Token);

        // Subscribe to CacheItemAdded events
        _cache.CacheItemAdded += CacheItemAddedHandler;
    }

    public async ValueTask DisposeAsync()
    {
        // Unsubscribe CacheItemAdded events, allow the cache to be GC'd
        _cache.CacheItemAdded -= CacheItemAddedHandler;

        try
        {
            _monitorCancellation.Cancel();
            await _monitorTask;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"{CacheName} error stopping monitor");
        }
    }

    private void CacheItemAddedHandler(object? sender, CacheItemAddedEventArgs<TKey> e)
    {
        if (e.Count >= CacheHighWatermark)
        {
            _overflowSemaphore.Release();
        }
    }

    private async Task MonitorWithRestarts(int maxRestarts, TimeSpan monitorInterval, CancellationToken cancellationToken)
    {
        for (int restart = 0; restart <= maxRestarts && !cancellationToken.IsCancellationRequested; ++restart)
        {
            try
            {
                _logger.LogInformation($"{CacheName} monitor {(restart > 0 ? "re" : "")}started");

                await Monitor(monitorInterval, cancellationToken);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"{CacheName} monitor failed");

                if (restart == maxRestarts)
                {
                    _logger.LogError($"{CacheName} monitor exceeded max retries");
                    throw;
                }

                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        _logger.LogInformation($"{CacheName} monitor stopped");
    }

    private async Task Monitor(TimeSpan monitorInterval, CancellationToken cancellationToken)
    {
        DateTime utcNextExpiry = DateTime.MinValue;
        while (!cancellationToken.IsCancellationRequested)
        {
            DateTime utcNow = DateTime.UtcNow;
            if (utcNextExpiry < utcNow)
            {
                utcNextExpiry = utcNow.RoundUp(monitorInterval);
            }

            _logger.LogDebug($"{CacheName} next expiry check: {utcNextExpiry}");

            if (await _overflowSemaphore.WaitAsync(utcNextExpiry - utcNow, cancellationToken))
            {
                _logger.LogDebug($"{CacheName} checking overflows");
                await ExpireOverflowsAsync(cancellationToken);
            }
            else
            {
                _logger.LogDebug($"{CacheName} checking expiries: {utcNextExpiry}");
                await ExpireTimeoutsAsync(utcNextExpiry, cancellationToken);
                utcNextExpiry += monitorInterval;
            }
        }
    }

    private async Task ExpireTimeoutsAsync(DateTime utcNow, CancellationToken cancellationToken)
    {
        if (CacheTimeout <= TimeSpan.Zero)
        {
            return;
        }

        DateTime utcThreshold = utcNow - CacheTimeout;

        List<KeyValuePair<TKey, TValue>> items = await _cache
            .RemoveOlderThanAsync(utcThreshold, cancellationToken)
            .ToListAsync();

        if (items.Any())
        {
            _logger.LogInformation($"{CacheName} expiring {items.Count} item{(items.Count == 1 ? "" : "s")} older than {utcThreshold}...");
        }

        await ExpireItemsAsync(items);
    }

    private async Task ExpireOverflowsAsync(CancellationToken cancellationToken)
    {
        int cacheSize = _cache.Count;
        if (cacheSize <= CacheHighWatermark)
        {
            return;
        }

        int itemsToRemove = _cache.Count - CacheLowWatermark;

        List<KeyValuePair<TKey, TValue>> items = await _cache
            .RemoveOldestAsync(itemsToRemove, cancellationToken)
            .ToListAsync();

        if (items.Any())
        {
            _logger.LogInformation($"{CacheName} size ({cacheSize}) exceeded high watermark ({CacheHighWatermark}), expiring {items.Count} items...");
        }
        else
        {
            _logger.LogWarning($"{CacheName} size ({cacheSize}) exceeded high watermark ({CacheHighWatermark}) but failed to remove any items");
        }

        await ExpireItemsAsync(items);
    }

    private async Task ExpireItemsAsync(IEnumerable<KeyValuePair<TKey, TValue>> removed)
    {
        foreach (var kvp in removed)
        {
            _logger.LogInformation($"{CacheName} expired: {kvp.Key}");

            if (kvp.Value is IAsyncDisposable disposable)
            {
                await disposable.DisposeAsync();
            }
            else
            {
                (kvp.Value as IDisposable)?.Dispose();
            }
        }
    }

    public override string ToString() => $"{nameof(AsyncCacheMonitor<TKey,TValue>)}:{CacheName}";
}