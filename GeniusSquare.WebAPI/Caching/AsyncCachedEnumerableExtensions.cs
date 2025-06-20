namespace GeniusSquare.WebAPI.Caching;

internal static class AsyncCachedEnumerableExtensions
{
    public static IAsyncCachedEnumerable<T> ToAsyncCachedEnumerable<T>(this IEnumerable<T> source) => new AsyncCachedEnumerable<T>(source.ToAsyncEnumerable());

    public static IAsyncCachedEnumerable<T> ToAsyncCachedEnumerable<T>(this IAsyncEnumerable<T> source) => new AsyncCachedEnumerable<T>(source);
}
