namespace GeniusSquare.WebAPI.Model;

public record Piece
{
    public required string Id { get; init; }

    public string? ConsoleColor { get; init; }
    public string? HtmlColor { get; init; }

    public List<Coord> Positions { get; init; } = [];
}
