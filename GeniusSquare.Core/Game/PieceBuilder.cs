using GeniusSquare.Core.Coords;
using System.Drawing;

namespace GeniusSquare.Core.Game;

public sealed class PieceBuilder
{
    private string _name;
    private ConsoleColor _consoleColor;
    private Color _htmlColor;
    
    private IList<Coord> _positions = [];
    private IList<Orientation> _orientations = [];

    public static PieceBuilder Create(string name, ConsoleColor consoleColor = default, Color htmlColor = default) =>
        new(name, consoleColor, htmlColor);

    private PieceBuilder(string name, ConsoleColor consoleColor, Color htmlColor)
    {
        _name = name;
        _consoleColor = consoleColor;
        _htmlColor = htmlColor;
    }

    public PieceBuilder WithPositions(IEnumerable<Coord> positions)
    {
        _positions = positions.ToList();
        return this;
    }

    public PieceBuilder WithOrientations(PieceOrientation pieceOrientation)
    {
        _orientations = pieceOrientation switch
        {
            PieceOrientation.Original => Orientations.Original,
            PieceOrientation.Rotate => Orientations.Rotations,
            PieceOrientation.RotateAndReflect => Orientations.RotationsAndReflections,
            _ => throw new ArgumentException($"Invalid {nameof(PieceOrientation)}: {pieceOrientation}", nameof(pieceOrientation))
        };
        return this;
    }

    public Piece BuildPiece()
    {
        Piece piece = new(_name, _consoleColor, _htmlColor);
        
        piece.AddOrientations(
            _orientations
                .Select(orientation => BuildOrientedPiece(piece, orientation))
                .Distinct(new PositionsComparer<OrientedPiece>()) // exclude duplicate orientations due to symmetry
        );
        
        return piece;
    }

    private OrientedPiece BuildOrientedPiece(Piece piece, Orientation orientation)
    {
        return new OrientedPiece(
            piece,
            orientation,
            _positions.Transform(orientation)
        );
    }
}
