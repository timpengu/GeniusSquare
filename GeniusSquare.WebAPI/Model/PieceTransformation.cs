using System.Text.Json.Serialization;

namespace GeniusSquare.WebAPI.Model;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PieceTransformation
{
    None,
    Rotate,
    RotateAndReflect,
}
