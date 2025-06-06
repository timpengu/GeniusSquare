using GeniusSquare.WebAPI.Helpers;

namespace GeniusSquare.WebAPI.Model;

public record OrientedPiece : INormal<OrientedPiece>
{
    public required string PieceId { get; init; }
    public required string OrientationId { get; init; }

    public List<Coord> Positions { get; init; } = [];

    public OrientedPiece Normalise() => this with
    {
        PieceId = PieceId.NormaliseId(),
        OrientationId = OrientationId.NormaliseId()
    };
}
