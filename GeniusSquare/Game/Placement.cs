using GeniusSquare.Coords;
using System.Xml.Linq;

namespace GeniusSquare.Game;

public record struct Placement(OrientedPiece OrientedPiece, Coord Coord)
{
    // TODO: Allow denormalised OrientedPiece to simplify adding Coord offset
    public override string ToString() => $"{OrientedPiece.Name,-4} @ {Coord} => [{string.Join(",", OrientedPiece.Positions.Offset(Coord))}]";
}
