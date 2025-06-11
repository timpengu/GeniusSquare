using GeniusSquare.Core.Coords;

namespace GeniusSquare.Core.Game;

public interface IHasPositions
{
    public IEnumerable<Coord> Positions { get; }
}
