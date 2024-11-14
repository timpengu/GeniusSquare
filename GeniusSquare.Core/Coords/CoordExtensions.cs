using System;
using System.Text.RegularExpressions;

namespace GeniusSquare.Core.Coords;

public static class CoordExtensions
{
    public static IEnumerable<Coord> Transpose(this IEnumerable<Coord> coords, Coord offset)
        => coords
            .Select(coord => coord + offset);

    public static CoordRange GetBounds(this IEnumerable<Coord> coords)
        => coords
            .ThrowIfEmpty()
            .Aggregate(
                new CoordRange(Coord.MaxValue, Coord.MinValue),
                (range, coord) => new CoordRange(
                    Coord.Min(range.Start, coord),
                    Coord.Max(range.End, coord + new Coord(1, 1)) // use exclusive range end
                ));

    public static IEnumerable<Coord> ToCoords(this IEnumerable<string> indexes) => indexes.Select(ToCoord);
    public static Coord ToCoord(this string index)
    {
        var match = Regex.Match(index, @"^([a-zA-Z])([0-9]+)$");
        if (!match.Success)
            throw new FormatException($"Invalid index string '{index}'");

        int x = Int32.Parse(match.Groups[2].Value) - 1; // 1-based numeric index
        int y = match.Groups[1].Value.ToUpper().Single() - 'A'; // A-based alphabetic index

        return new Coord(x, y);
    }
}