using GeniusSquare.Core.Coords;

namespace GeniusSquare.Core.Game;

public record struct Placement(OrientedPiece OrientedPiece, Coord Offset)
{
    public IEnumerable<Coord> Positions => OrientedPiece.Positions.Transpose(Offset);

    public override string ToString() => $"{OrientedPiece.Name,-4} @ {Offset} => [{string.Join(",", Positions)}]";
}
