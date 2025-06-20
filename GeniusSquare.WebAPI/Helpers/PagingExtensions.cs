namespace GeniusSquare.WebAPI.Helpers;

internal static class PagingExtensions
{
    public static IEnumerable<T> Skip<T>(this IEnumerable<T> source, int? skip) =>
        skip.HasValue ? Enumerable.Skip(source, skip.Value) : source;

    public static IEnumerable<T> Top<T>(this IEnumerable<T> source, int? top) =>
        top.HasValue ? Enumerable.Take(source, top.Value) : source;

    public static IAsyncEnumerable<T> Skip<T>(this IAsyncEnumerable<T> source, int? skip) =>
        skip.HasValue ? AsyncEnumerable.Skip(source, skip.Value) : source;

    public static IAsyncEnumerable<T> Top<T>(this IAsyncEnumerable<T> source, int? top) =>
        top.HasValue ? AsyncEnumerable.Take(source, top.Value) : source;
}
