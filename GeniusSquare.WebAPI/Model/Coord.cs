using GeniusSquare.WebAPI.Model.Serialization;
using System.Text.Json.Serialization;

using Domain = GeniusSquare.Core.Coords;
namespace GeniusSquare.WebAPI.Model;

[JsonConverter(typeof(CoordArrayJsonConverter))]
public record struct Coord(int X, int Y)
{
    public static implicit operator Coord((int X, int Y) value) => new Coord(value.X, value.Y);

    // TODO: Use an AutoMapper framework to convert betwen WebAPI and domain models?
    public static explicit operator Coord(Domain.Coord coord) => new(coord.X, coord.Y);
    public static explicit operator Domain.Coord(Coord coord) => new(coord.X, coord.Y);
}
