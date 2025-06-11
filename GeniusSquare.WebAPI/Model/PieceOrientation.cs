using System.Text.Json.Serialization;

namespace GeniusSquare.WebAPI.Model;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PieceOrientation
{
    Original,
    Rotate,
    RotateAndReflect,
}
