namespace GeniusSquare.Configuration;

public sealed record ConfigPiece
{
    public string? Name { get; set; }
    public ConsoleColor ConsoleColor { get; set; } = ConsoleColor.Black;
    public ConfigCoord[] Positions { get; set; } = [];
}