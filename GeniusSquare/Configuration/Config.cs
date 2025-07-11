﻿using GeniusSquare.Core.Game;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GeniusSquare.Configuration;

public sealed record Config
{
    public ConfigCoord? DefaultBoardSize { get; set; }
    
    public ConfigPiece[] Pieces { get; set; } = [];

    public PieceOrientation PieceOrientation { get; set; } = PieceOrientation.Original;

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
