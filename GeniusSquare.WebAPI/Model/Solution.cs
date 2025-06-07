namespace GeniusSquare.WebAPI.Model;

public record Solution
{
    public required string ConfigId { get; init; }
    public required int Index { get; init; }

    public List<Coord> OccupiedPositions { get; init; } = [];
    public List<Placement> Placements { get; init; } = [];
}
