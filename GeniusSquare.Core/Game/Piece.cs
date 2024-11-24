using GeniusSquare.Core.Coords;

namespace GeniusSquare.Core.Game;

public sealed class Piece
{
    // TODO: Make constructor private/internal? Use factory method or InternalsVisibleTo for tests.
    public Piece(string name, ConsoleColor consoleColor)
    {
        Name = name;
        ConsoleColor = consoleColor;
    }

    internal void AddOrientations(IEnumerable<OrientedPiece> orientations)
    {
        _orientations.AddRange(orientations.ThrowIfEmpty());
        Positions = _orientations.Select(o => o.Positions.Count).Distinct().Single(); // all orientations must have same positions count
    }

    public string Name { get; }
    public ConsoleColor ConsoleColor { get; }
    public int Positions { get; private set; } = 0;

    public IReadOnlyList<OrientedPiece> Orientations => _orientations;
    private List<OrientedPiece> _orientations = new();

    public override string ToString() => Name;
}
