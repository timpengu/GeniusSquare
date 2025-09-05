using GeniusSquare.Core.Coords;

namespace GeniusSquare.Core.Game;

public sealed record BoardSize(int XSize, int YSize) : IBoardSize
{
    public static BoardSize FromCoord(Coord size) => new(size.X, size.Y);
}
