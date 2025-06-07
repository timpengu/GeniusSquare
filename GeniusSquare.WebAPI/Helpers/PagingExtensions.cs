namespace GeniusSquare.WebAPI.Helpers;

public static class PagingExtensions
{
    public static IEnumerable<T> Skip<T>(this IEnumerable<T> source, int? skip) => skip.HasValue ? source.Skip(skip.Value) : source;
    public static IEnumerable<T> Top<T>(this IEnumerable<T> source, int? top) => top.HasValue ? source.Take(top.Value) : source;
}