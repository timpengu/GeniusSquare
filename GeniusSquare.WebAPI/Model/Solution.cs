namespace GeniusSquare.WebAPI.Model;

public record Solution
{
    public required string ConfigId { get; init; }
    public required int SolutionNumber { get; init; }
    
    public List<Placement> Placements { get; init; } = [];
    public string[][] LayoutPieceIds { get; init; } = [];
}
