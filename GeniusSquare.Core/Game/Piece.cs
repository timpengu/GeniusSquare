using GeniusSquare.Core.Coords;

namespace GeniusSquare.Core.Game;
public sealed record Piece
{
    public Piece(string name, IEnumerable<OrientedPiece> orientations)
    {
        Name = name;
        Orientations = orientations.ThrowIfEmpty().ToList();
    }

    public string Name { get; }
    public IReadOnlyList<OrientedPiece> Orientations { get; }

    public int Positions => Orientations[0].Positions.Count;
    public override string ToString() => Name;
}
