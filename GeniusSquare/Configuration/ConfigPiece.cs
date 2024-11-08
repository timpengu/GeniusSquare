namespace GeniusSquare.Configuration;

public sealed record ConfigPiece
{
    public string? Name { get; set; }
    public ConfigCoord[] Positions { get; set; } = [];
}