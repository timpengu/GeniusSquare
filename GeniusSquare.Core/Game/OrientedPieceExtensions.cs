﻿using GeniusSquare.Core.Coords;

namespace GeniusSquare.Core.Game;
public static class OrientedPieceExtensions
{
    public static CoordRange GetPlacementRange(this OrientedPiece piece, Board board)
    {
        Coord placementStart = board.Bounds.Start - piece.Bounds.Start;
        Coord placementEnd = board.Bounds.End - piece.Bounds.End + new Coord(1, 1); // use exclusive range end
        placementEnd = Coord.Max(placementStart, placementEnd); // avoid negative range if piece is larger than board
        return new CoordRange(placementStart, placementEnd);
    }

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