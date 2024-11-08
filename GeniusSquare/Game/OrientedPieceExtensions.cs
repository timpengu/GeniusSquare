using GeniusSquare.Coords;

namespace GeniusSquare.Game;
public static class OrientedPieceExtensions
{
    public static CoordRange GetPlacementRange(this OrientedPiece piece, Board board) =>
        new CoordRange(
            board.Bounds.Start - piece.Bounds.Start,
            board.Bounds.End - piece.Bounds.End + new Coord(1,1) // adjust range end to be exclusive
        );

    public static OrientedPiece Reflect(this OrientedPiece piece, string? name = null) =>
        new OrientedPiece(
            name ?? piece.Name,
            piece.Positions.Select(pos => new Coord(-pos.X, pos.Y))
        );

    public static OrientedPiece Rotate(this OrientedPiece piece, string? name = null) =>
        new OrientedPiece(
            name ?? piece.Name,
            piece.Positions.Select(pos => new Coord(-pos.Y, pos.X))
        );
}
