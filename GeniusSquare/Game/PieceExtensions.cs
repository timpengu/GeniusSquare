using System.Linq;
using GeniusSquare.Coords;

namespace GeniusSquare.Game;
public static class PieceExtensions
{
    public static IEnumerable<Placement> GetPlacements(this Piece piece, Board board) =>
        from OrientedPiece orientedPiece in piece.Orientations
        from Coord coord in orientedPiece
            .GetPlacementRange(board.CoordRange)
            .EnumerateCoords()
            .Where(coord => board.CanPlace(orientedPiece, coord))
        select new Placement(orientedPiece, coord);
}
