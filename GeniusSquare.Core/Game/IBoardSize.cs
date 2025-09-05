using GeniusSquare.Core.Coords;

namespace GeniusSquare.Core.Game;

public interface IBoardSize
{
    int XSize { get; }
    int YSize { get; }

    // TODO: define Size/Bounds as extension properties in C#14 ?
    public sealed Coord Size => new Coord(XSize, YSize);
    public sealed CoordRange Bounds => new(Coord.Zero, Size);
}
