using GeniusSquare.WebAPI.Helpers;

namespace GeniusSquare.WebAPI.Model;

public record Piece : INormal<Piece>
{
    public required string Id { get; init; }

    public string? ConsoleColor { get; init; }
    public string? HtmlColor { get; init; }

    public List<Coord> Positions { get; init; } = [];

    public Piece Normalise() => this with
    {
        Id = Id.NormaliseId()
    };
}
