using GeniusSquare.WebAPI.Helpers;

namespace GeniusSquare.WebAPI.Model;

public record Piece : INormal<Piece>
{
    public required string PieceId { get; init; }

    public List<Coord> Positions { get; init; } = [];
    public PieceAttributes Attributes { get; init; } = new();

    public Piece Normalise() => this with
    {
        PieceId = PieceId.NormaliseId()
    };
}
