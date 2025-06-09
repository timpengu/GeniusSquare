using GeniusSquare.Core.Coords;

namespace GeniusSquare.Core.Game;

public sealed class PieceBuilder
{
    private record struct NamedOrientation(Orientation Orientation, string Suffix) { }

    private static class Orientations
    {
        public static List<NamedOrientation> Original =
        [
            new (Orientation.Ar, "a"),
        ];

        public static List<NamedOrientation> Rotated =
        [
            new (Orientation.Ar, "a"),
            new (Orientation.Br, "b"),
            new (Orientation.Cr, "c"),
            new (Orientation.Dr, "d"),
        ];

        public static List<NamedOrientation> RotatedAndReflected =
        [
            new (Orientation.Ar, "ar"),
            new (Orientation.Br, "br"),
            new (Orientation.Cr, "cr"),
            new (Orientation.Dr, "dr"),
            new (Orientation.Al, "al"),
            new (Orientation.Bl, "bl"),
            new (Orientation.Cl, "cl"),
            new (Orientation.Dl, "dl"),
        ];
    }

    private string _name;
    private ConsoleColor _consoleColor;
    private List<Coord> _positions = [];
    private List<NamedOrientation> _orientations = [];

    public static PieceBuilder Create(string name, ConsoleColor consoleColor) => new(name, consoleColor);
    private PieceBuilder(string name, ConsoleColor consoleColor)
    {
        _name = name;
        _consoleColor = consoleColor;
    }

    public PieceBuilder WithPositions(IEnumerable<Coord> positions)
    {
        _positions = positions.ToList();
        return this;
    }

    public PieceBuilder WithOrientations(PieceTransformation transformation)
    {
        _orientations = transformation switch
        {
            PieceTransformation.None => Orientations.Original,
            PieceTransformation.Rotate => Orientations.Rotated,
            PieceTransformation.RotateAndReflect => Orientations.RotatedAndReflected,
            _ => throw new ArgumentException($"Invalid {nameof(PieceTransformation)}: {transformation}", nameof(transformation))
        };
        return this;
    }

    public Piece BuildPiece()
    {
        Piece piece = new(_name, _consoleColor);
        
        piece.AddOrientations(
            _orientations
                .Select(orientation => BuildOrientedPiece(piece, orientation))
                .Distinct() // exclude duplicate orientations due to symmetry
        );
        
        return piece;
    }

    private OrientedPiece BuildOrientedPiece(Piece piece, NamedOrientation orientation)
    {
        return new OrientedPiece(
            piece,
            orientation.Orientation,
            piece.Name + orientation.Suffix,
            _positions.Transform(orientation.Orientation)
        );
    }
}
