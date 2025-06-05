using System.Text.Json.Serialization;
using System.Text.Json;

namespace GeniusSquare.WebAPI.Model.Serialization;

/// <summary>
/// Converts a <see cref="Coord"/> to and from a JSON array of two integers.
/// </summary>
public class CoordArrayJsonConverter : JsonConverter<Coord>
{
    private readonly static JsonConverter<int[]> _arrayConverter = (JsonConverter<int[]>)JsonSerializerOptions.Default.GetConverter(typeof(int[]));

    public override void Write(Utf8JsonWriter writer, Coord value, JsonSerializerOptions options)
    {
        _arrayConverter.Write(writer, [value.X, value.Y], options);
    }

    public override Coord Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (typeToConvert != typeof(Coord))
        {
            throw new NotSupportedException($"Unsupported type: {typeToConvert.Name}");
        }

        int[]? values = _arrayConverter.Read(ref reader, typeof(int[]), options);

        if (values == null)
        {
            throw new JsonException($"The JSON value could not be converted to {nameof(Coord)}: null array");
        }
        if (values.Length != 2)
        {
            throw new JsonException($"The JSON value could not be converted to {nameof(Coord)}: array length {values?.Length}");
        }

        return new Coord(values[0], values[1]);
    }
}
