using GeniusSquare.Coords;
using GeniusSquare.Game;

namespace GeniusSquare.Config;

public static class ConfigPiecesExtensions
{
    public static IEnumerable<Piece> GeneratePieces(this ConfigPieces config)
    {
        foreach (ConfigPiece pieceConfig in config.Pieces)
        {
            if (pieceConfig.Name == null)
                throw new Exception("Piece has null Name");

            Coord ToCoord(int[] coords) =>
                coords.Length == 2
                    ? new Coord(coords[0], coords[1])
                    : throw new Exception($"Piece {pieceConfig.Name} has invalid coords length: ({string.Join(",", coords)})");

            OrientedPiece basePiece = new(pieceConfig.Name, pieceConfig.Positions.Select(ToCoord));
            List<OrientedPiece> orientations = new() { basePiece };

            if (config.AllowRotation)
            {
                OrientedPiece rotatedPiece = basePiece;
                for (int rotation = 1; rotation < 4; ++rotation)
                {
                    rotatedPiece = rotatedPiece.Rotate($"{basePiece.Name}{rotation}");
                    orientations.Add(rotatedPiece);
                }
            }

            if (config.AllowReflection)
            {
                List<OrientedPiece> reflectedPieces = orientations
                    .Select(piece => piece.Reflect($"{piece.Name}R"))
                    .ToList();

                orientations.AddRange(reflectedPieces);
            }

            yield return new Piece(pieceConfig.Name, orientations.Distinct());
        }
    }
}
