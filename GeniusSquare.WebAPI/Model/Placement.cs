namespace GeniusSquare.WebAPI.Model;

public record Placement
{
    public required string PieceId { get; init; }
    public required string OrientationId { get; init; }
    public required Coord Offset { get; init; }
}
