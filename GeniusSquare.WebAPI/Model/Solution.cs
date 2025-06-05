namespace GeniusSquare.WebAPI.Model;

public record Solution(
    int Index,
    string ConfigId,
    List<Coord> OccupiedPositions,
    List<Placement> Placements)
{
}
