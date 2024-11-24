using System;
using System.Text.RegularExpressions;

namespace GeniusSquare.Core.Coords;

public static class CoordExtensions
{
    public static Coord ReflectX(this Coord coord) => new(-coord.X, coord.Y);
    public static Coord ReflectY(this Coord coord) => new(coord.X, -coord.Y);
    public static Coord Rotate90(this Coord coord) => new(-coord.Y, coord.X);
    public static Coord Rotate180(this Coord coord) => new(-coord.X, -coord.Y);
    public static Coord Rotate270(this Coord coord) => new(coord.Y, -coord.X);

    public static IEnumerable<Coord> ReflectX(this IEnumerable<Coord> coords) => coords.Select(ReflectX);
    public static IEnumerable<Coord> ReflectY(this IEnumerable<Coord> coords) => coords.Select(ReflectY);
    public static IEnumerable<Coord> Rotate90(this IEnumerable<Coord> coords) => coords.Select(Rotate90);
    public static IEnumerable<Coord> Rotate180(this IEnumerable<Coord> coords) => coords.Select(Rotate180);
    public static IEnumerable<Coord> Rotate270(this IEnumerable<Coord> coords) => coords.Select(Rotate270);

    public static IEnumerable<Coord> Transpose(this IEnumerable<Coord> coords, Coord offset) => coords.Select(coord => coord + offset);

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