using System.Text.Json;

namespace GeniusSquare.Configuration;

public sealed record Config
{
    public ConfigCoord? BoardSize { get; set; }
    
    public ConfigCoord[] OccupiedCoords { get; set; } = [];
    public string[] OccupiedIndexes { get; set; } = [];
    public int OccupiedRandoms { get; set; } = 0;

    public ConfigPiece[] Pieces { get; set; } = [];

    public bool AllowRotation { get; set; } = true;
    public bool AllowReflection { get; set; } = true;

    public static Config Load(string configPath)
    {
        string json = File.ReadAllText(configPath);

        JsonSerializerOptions options = new() { ReadCommentHandling = JsonCommentHandling.Skip };
        Config? config = JsonSerializer.Deserialize<Config>(json, options);

        return config ?? throw new Exception($"Failed to deserialize {nameof(Config)}");
    }
}