using System;
using System.Text.RegularExpressions;

namespace GeniusSquare.Coords;

public static class CoordExtensions
{
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

    public static IEnumerable<Coord> Offset(this IEnumerable<Coord> positions, Coord offset) => positions.Select(pos => pos + offset);

    // TODO: Convert GetBounds to CoordRange ctor?
    public static CoordRange GetBounds(this IEnumerable<Coord> coords) =>
        coords
        .ThrowIfEmpty()
        .Aggregate(
            new CoordRange(
                new(int.MaxValue, int.MaxValue),
                new(int.MinValue, int.MinValue)
            ),
            (range, coord) => new CoordRange(
                new(
                    Math.Min(range.Start.X, coord.X),
                    Math.Min(range.Start.Y, coord.Y)),
                new(
                    Math.Max(range.End.X, coord.X + 1),
                    Math.Max(range.End.Y, coord.Y + 1))
            ));

    // TODO: Convert EnumerateCoords etc into member methods?
    public static IEnumerable<Coord> EnumerateCoords(this CoordRange range)
    {
        foreach (int y in range.EnumerateY())
        {
            foreach (int x in range.EnumerateX())
            {
                yield return new Coord(x, y);
            }
        }
    }

    public static IEnumerable<int> EnumerateX(this CoordRange range) => Range(range.Start.X, range.End.X);
    public static IEnumerable<int> EnumerateY(this CoordRange range) => Range(range.Start.Y, range.End.Y);
    private static IEnumerable<int> Range(int start, int end) => Enumerable.Range(start, end - start);
}

