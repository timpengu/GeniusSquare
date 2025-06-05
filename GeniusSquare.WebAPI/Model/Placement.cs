namespace GeniusSquare.WebAPI.Model;

public record Placement(
    string PieceId,
    string OrientationId,
    Coord Offset)
{
}
