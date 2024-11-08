using GeniusSquare.Coords;
using GeniusSquare.Game;

namespace GeniusSquare.Configuration;

public static class ConfigExtensions
{
    public static Coord ToCoord(this ConfigCoord coord) => new Coord(coord.X, coord.Y);

    public static Board GenerateBoard(this Config config)
    {
        try
        {
            Coord boardSize = config.BoardSize?.ToCoord()
                ?? throw new Exception($"Config has missing {nameof(Config.BoardSize)}.");

            return Board
                .Create(boardSize)
                .WithOccupied(config.OccupiedIndexes)
                .WithOccupied(config.OccupiedCoords.Select(ToCoord));
        }
        catch (Exception e)
        {
            throw new Exception("Failed to load board from config.", e);
        }
    }

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
        string pieceName = configPiece.Name ?? String.Empty;

        char rotationSuffix = 'a';
        char reflectionSuffix = '\'';

        string basePieceName = config.AllowRotation ? pieceName + rotationSuffix : pieceName;
        OrientedPiece basePiece = new(basePieceName, configPiece.Positions.Select(ToCoord));

        List<OrientedPiece> orientations = new() { basePiece };

        if (config.AllowRotation)
        {
            OrientedPiece rotatedPiece = basePiece;
            for (int rotation = 1; rotation < 4; ++rotation)
            {
                ++rotationSuffix; // label rotations 'a','b','c','d'
                rotatedPiece = rotatedPiece.Rotate(pieceName + rotationSuffix);
                orientations.Add(rotatedPiece);
            }
        }

        if (config.AllowReflection)
        {
            List<OrientedPiece> reflectedPieces = orientations
                .Select(piece => piece.Reflect(piece.Name + reflectionSuffix))
                .ToList();

            orientations.AddRange(reflectedPieces);
        }

        return new Piece(
            pieceName,
            orientations.Distinct() // exclude duplicates due to symmetry (instances must be normalised)
        );
    }
}
