using GeniusSquare.Core.Coords;
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
                piece = configPiece.GeneratePiece(config.Transformation);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to load piece '{configPiece.Name}' from config.", e);
            }

            yield return piece;
        }
    }

    private static Piece GeneratePiece(this ConfigPiece configPiece, Transformation transformation)
    {
        var builder = PieceBuilder
            .Create(configPiece.Name ?? "", configPiece.ConsoleColor)
            .WithPositions(configPiece.Positions.Select(ToCoord));

        if (transformation.HasFlag(Transformation.Rotate))
        {
            builder.AddRotations();
        }

        if (transformation.HasFlag(Transformation.ReflectX))
        {
            builder.AddReflectionsX();
        }

        if (transformation.HasFlag(Transformation.ReflectY))
        {
            builder.AddReflectionsY();
        }

        return builder.BuildPiece();
    }
}
