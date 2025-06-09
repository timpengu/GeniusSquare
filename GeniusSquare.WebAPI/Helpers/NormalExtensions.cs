using GeniusSquare.WebAPI.Model;
using System.Globalization;

namespace GeniusSquare.WebAPI.Helpers;

internal static class NormalExtensions
{
    public static string NormaliseId(this string value) =>
        value.Trim().ToLower(CultureInfo.InvariantCulture);

    public static IEnumerable<T> Normalise<T>(this IEnumerable<T> source) where T : INormal<T> =>
        source.Select(t => t.Normalise());
}
