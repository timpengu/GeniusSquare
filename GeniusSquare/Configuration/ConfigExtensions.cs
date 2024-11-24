﻿using GeniusSquare.Core.Coords;
using GeniusSquare.Core.Game;

namespace GeniusSquare.Configuration;

public static class ConfigExtensions
{
    public static Coord? GetDefaultBoardSize(this Config config) => config.DefaultBoardSize?.ToCoord();
    public static Coord ToCoord(this ConfigCoord coord) => new Coord(coord.X, coord.Y);

    public static IEnumerable<Piece> GeneratePieces(this Config config)
    {
        for (int i = 0; i < config.Pieces.Length; i++)
        {
            ConfigPiece configPiece = config.Pieces[i];
            if (configPiece.Name == null)
            {
                throw new Exception($"Pieces index [{i}] has missing {nameof(ConfigPiece.Name)}.");
            }

            Piece piece;
            try
            {
                piece = config.GeneratePiece(configPiece);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to load piece '{configPiece.Name}' from config.", e);
            }

            yield return piece;
        }
    }

    private static Piece GeneratePiece(this Config config, ConfigPiece configPiece)
    {
        var builder = PieceBuilder
            .Create(configPiece.Name ?? "", configPiece.ConsoleColor)
            .WithPositions(configPiece.Positions.Select(ToCoord));

        if (config.AllowRotation)
        {
            builder.AddRotations();
        }

        if (config.AllowXReflection)
        {
            builder.AddXReflections();
        }

        if (config.AllowYReflection)
        {
            builder.AddYReflections();
        }

        return builder.BuildPiece();
    }
}
