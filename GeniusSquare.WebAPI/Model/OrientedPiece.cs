namespace GeniusSquare.WebAPI.Model;

public record OrientedPiece(
    string Id,
    string PieceId,
    List<Coord> Positions)
{
}
