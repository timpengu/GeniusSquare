namespace GeniusSquare.WebAPI.Caching;

public sealed class CacheItemAddedEventArgs<TKey>(TKey key, int count) : EventArgs
{
    public TKey Key { get; } = key;
    public int Count { get; } = count;
}
