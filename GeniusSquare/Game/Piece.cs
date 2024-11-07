namespace GeniusSquare.Game;
public sealed record Piece
{
    public Piece(string name, IEnumerable<OrientedPiece> orientations)
    {
        Name = name;
        Orientations = orientations.ToList();
    }

    public string Name { get; }
    public IReadOnlyList<OrientedPiece> Orientations { get; }

    public override string ToString() => Name;
}
