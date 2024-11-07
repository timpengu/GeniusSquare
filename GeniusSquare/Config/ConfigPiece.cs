namespace GeniusSquare.Config;

public sealed record ConfigPiece
{
    public string? Name { get; set; }
    public int[][] Positions { get; set; } = [];
}