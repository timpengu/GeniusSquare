namespace GeniusSquare.WebAPI.Model;

public record OrientedPiece
{
    public required string PieceId { get; init; }
    public required string OrientationId { get; init; }

    public List<Coord> Positions { get; init; } = [];
}
