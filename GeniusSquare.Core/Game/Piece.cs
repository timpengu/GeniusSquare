using GeniusSquare.Core.Coords;

namespace GeniusSquare.Core.Game;
public sealed record Piece
{
    public Piece(string name, ConsoleColor consoleColor, IEnumerable<OrientedPiece> orientations)
    {
        Name = name;
        ConsoleColor = consoleColor;
        Orientations = orientations.ThrowIfEmpty().ToList();
        Positions = Orientations.Select(o => o.Positions.Count).Distinct().Single(); // all orientations must have same number of positions
    }

    public string Name { get; }
    public ConsoleColor ConsoleColor { get; }
    public int Positions { get; }
    public IReadOnlyList<OrientedPiece> Orientations { get; }

    public override string ToString() => Name;
}
