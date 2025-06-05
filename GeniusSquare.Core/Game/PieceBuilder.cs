using GeniusSquare.Core.Coords;

namespace GeniusSquare.Core.Game;

public sealed class PieceBuilder
{
    private record struct Orientation(string Suffix, List<Coord> Positions)
    {
        public Orientation Transform(char suffix, Func<Coord, Coord> transformation) =>
            new Orientation(
                Suffix + suffix,
                Positions.Select(transformation).ToList());
    }

    private string _name;
    private ConsoleColor _consoleColor;
    private List<Orientation> _orientations = new();

    public static PieceBuilder Create(string name, ConsoleColor consoleColor) => new(name, consoleColor);
    private PieceBuilder(string name, ConsoleColor consoleColor)
    {
        _name = name;
        _consoleColor = consoleColor;
    }

    public PieceBuilder WithPositions(IEnumerable<Coord> positions) => WithPositions(String.Empty, positions);
    public PieceBuilder WithPositions(string suffix, IEnumerable<Coord> positions)
    {
        Orientation orientation = new(suffix, positions.ToList());
        _orientations.Add(orientation);
        return this;
    }

    public PieceBuilder AddRotations() =>
        ReplaceWithTransformations(orientation => [
            orientation.Transform('a', coord => coord),
            orientation.Transform('b', coord => coord.Rotate90()),
            orientation.Transform('c', coord => coord.Rotate180()),
            orientation.Transform('d', coord => coord.Rotate270())
        ]);

    public PieceBuilder AddReflectionsX() =>
        AddTransformations(orientation => [
            orientation.Transform('x', coord => coord.ReflectX())
        ]);

    public PieceBuilder AddReflectionsY() =>
        AddTransformations(orientation => [
            orientation.Transform('y', coord => coord.ReflectY())
        ]);

    private PieceBuilder ReplaceWithTransformations(Func<Orientation, IEnumerable<Orientation>> transformations)
    {
        _orientations = _orientations.SelectMany(transformations).ToList();
        return this;
    }

    private PieceBuilder AddTransformations(Func<Orientation, IEnumerable<Orientation>> transformations)
    {
        var newOrientations = _orientations.SelectMany(transformations).ToList();
        _orientations.AddRange(newOrientations);
        return this;
    }

    public Piece BuildPiece()
    {
        Piece piece = new(_name, _consoleColor);
        
        piece.AddOrientations(
            _orientations
                .Select(o => new OrientedPiece(piece, _name + o.Suffix, o.Positions))
                .Distinct() // exclude duplicated orientations
        );
        
        return piece;
    }
}
