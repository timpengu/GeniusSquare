using System.Text.Json.Serialization;

namespace GeniusSquare.WebAPI.Model;

// TODO: Unify with GeniusSquare.Configuration.Transformation?
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Transformation
{
    None = 0,
    ReflectX = 1,
    ReflectY = 2,
    ReflectXY = ReflectX | ReflectY,
    Rotate = 4,
    RotateReflect = Rotate | ReflectX,
}