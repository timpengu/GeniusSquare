
namespace GeniusSquare.WebAPI.Caching;

public interface IAsyncCacheMonitor : IAsyncDisposable
{
    string CacheName { get; }
    int CacheHighWatermark { get; }
    int CacheLowWatermark { get; }
    TimeSpan CacheTimeout { get; }
}
