namespace GeniusSquare.WebAPI.Model;

public record Config
{
    public static readonly string DefaultId = "default";

    public required string Id { get; init; }
    public required Coord BoardSize { get; init; }
    public required Transformation Transformation { get; init; }
    public List<string> Pieces { get; init; } = [];
}
