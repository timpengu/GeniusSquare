using GeniusSquare.Coords;
using System.Text;

namespace GeniusSquare.Game;
public sealed record Board
{
    private readonly Coord _bounds;
    private bool[,] _isOccupied; // TODO: can't make this readonly?

    public static Board Empty(int xBound, int yBound) => new Board(new Coord(xBound, yBound));
    private Board(Coord bounds)
    {
        _bounds = bounds;
        _isOccupied = new bool[bounds.X, bounds.Y];
    }

    public CoordRange CoordRange => new(Coord.Zero, _bounds);

    public bool IsOccupied(Coord position) => _isOccupied[position.X, position.Y];
    public bool CanPlace(OrientedPiece piece, Coord coord) => !piece.Positions.Offset(coord).Any(IsOccupied);

    public Board WithPositions(params string[] indexes) => WithPositions(indexes.ToCoords());
    public Board WithPositions(IEnumerable<Coord> positions) => this with { _isOccupied = WithOccupied(positions) };
    public Board WithPlacement(Placement placement) => this with { _isOccupied = WithOccupied(placement.OrientedPiece.Positions.Offset(placement.Coord)) };
    private bool[,] WithOccupied(IEnumerable<Coord> positions)
    {
        var isOccupied = (bool[,])_isOccupied.Clone();
        foreach (var position in positions)
        {
            isOccupied[position.X, position.Y] = true;
        }
        return isOccupied;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        foreach (int y in CoordRange.EnumerateY())
        {
            foreach (int x in CoordRange.EnumerateX())
            {
                sb.Append(
                    IsOccupied(new(x, y)) ? 'x' : '.'
                );
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }
}
