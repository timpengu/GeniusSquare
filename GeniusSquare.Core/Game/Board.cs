using GeniusSquare.Core.Coords;
using System.Text;

namespace GeniusSquare.Core.Game;

public sealed record Board : IBoardSize
{
    private readonly IEnumerable<Placement> _placements;
    private readonly bool[,] _occupation;

    public int XSize => _occupation.GetLength(0);
    public int YSize => _occupation.GetLength(1);
    public Coord Size { get; }
    public CoordRange Bounds { get; }

    /// <summary>
    /// Returns an empty board of the given size
    /// </summary>
    public static Board Create(Coord size) => new Board(size);

    /// <summary>
    /// Returns a board with additional occupied positions
    /// </summary>
    public Board WithOccupiedPositions(IEnumerable<Coord> positions) => new Board(_placements, WithOccupation(positions));

    /// <summary>
    /// Returns a board with placement of an oriented piece
    /// </summary>
    public Board WithPlacement(Placement placement) => new Board(_placements.Append(placement), WithOccupation(placement.Positions));

    public IReadOnlyCollection<Placement> Placements => _placements.ToList();

    public bool IsOccupied(Placement placement) => placement.Positions.Any(IsOccupied);
    public bool IsOccupied(Coord position) => _occupation[position.X, position.Y];

    private Board(Coord size)
    {
        if (size.X < 0 || size.Y < 0)
            throw new ArgumentException("Board size cannot be negative.", nameof(size));

        _placements = Enumerable.Empty<Placement>();
        _occupation = new bool[size.X, size.Y];
    }

    private Board(IEnumerable<Placement> placements, bool[,] occupation)
    {
        _placements = placements;
        _occupation = occupation;
    }

    private bool[,] WithOccupation(IEnumerable<Coord> positions)
    {
        bool[,] occupation = (bool[,])_occupation.Clone();
        foreach (Coord position in positions)
        {
            Validate(position);
            occupation[position.X, position.Y] = true;
        }
        return occupation;
    }

    private void Validate(Coord position)
    {
        if (position.X < 0 || position.X >= XSize ||
            position.Y < 0 || position.Y >= YSize)
        {
            throw new IndexOutOfRangeException($"Position {position} is outside board bounds {Bounds}");
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        foreach (int y in Bounds.EnumerateY())
        {
            foreach (int x in Bounds.EnumerateX())
            {
                if (x != Bounds.Start.X)
                {
                    sb.Append(' ');
                }
                else if (y != Bounds.Start.Y)
                {
                    sb.AppendLine();
                }

                sb.Append(
                    IsOccupied(new Coord(x, y)) ? 'x' : '.'
                );
            }
        }

        return sb.ToString();
    }
}
