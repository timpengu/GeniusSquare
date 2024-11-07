using System.Text.Json;

namespace GeniusSquare.Config;

public sealed record ConfigPieces
{
    public bool AllowRotation { get; set; }
    public bool AllowReflection { get; set; }
    public ConfigPiece[] Pieces { get; set; } = [];

    public static ConfigPieces Load(string configPath)
    {
        string json = File.ReadAllText(configPath);

        JsonSerializerOptions options = new() { ReadCommentHandling = JsonCommentHandling.Skip };
        ConfigPieces? piecesConfig = JsonSerializer.Deserialize<ConfigPieces>(json, options);

        return piecesConfig
            ?? throw new Exception($"Failed to deserialize {nameof(ConfigPieces)}");
    }
}