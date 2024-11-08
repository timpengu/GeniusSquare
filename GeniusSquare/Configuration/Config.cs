using System.Text.Json;

namespace GeniusSquare.Configuration;

public sealed record Config
{
    public ConfigCoord? BoardSize { get; set; }
    
    public ConfigCoord[] OccupiedCoords { get; set; } = [];
    public string[] OccupiedIndexes { get; set; } = [];

    public ConfigPiece[] Pieces { get; set; } = [];
    
    public bool AllowRotation { get; set; }
    public bool AllowReflection { get; set; }

    public static Config Load(string configPath)
    {
        string json = File.ReadAllText(configPath);

        JsonSerializerOptions options = new() { ReadCommentHandling = JsonCommentHandling.Skip };
        Config? config = JsonSerializer.Deserialize<Config>(json, options);

        return config ?? throw new Exception($"Failed to deserialize {nameof(Config)}");
    }
}