using GeniusSquare.WebAPI.Model.Serialization;
using System.Text.Json.Serialization;

namespace GeniusSquare.WebAPI.Model;

[JsonConverter(typeof(CoordArrayJsonConverter))]
public record struct Coord(int X, int Y)
{
    public static implicit operator Coord((int X, int Y) value) => new Coord(value.X, value.Y);
}
