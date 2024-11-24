﻿using System.Text.Json;
using System.Text.Json.Serialization;

namespace GeniusSquare.Configuration;

public sealed record Config
{
    public ConfigCoord? DefaultBoardSize { get; set; }
    
    public ConfigPiece[] Pieces { get; set; } = [];

    public bool AllowRotation { get; set; } = true;
    public bool AllowReflection { get; set; } = true;

    public static Config Load(string configPath)
    {
        string json = File.ReadAllText(configPath);

        JsonSerializerOptions options = new()
        {
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
            ReadCommentHandling = JsonCommentHandling.Skip,
        };
        Config? config = JsonSerializer.Deserialize<Config>(json, options);

        return config ?? throw new Exception($"Failed to deserialize {nameof(Config)}");
    }
}