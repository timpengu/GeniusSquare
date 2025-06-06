using GeniusSquare.WebAPI.Helpers;

namespace GeniusSquare.WebAPI.Model;

public record Config : INormal<Config>
{
    public static readonly string DefaultId = "default";

    public required string Id { get; init; }
    public required Coord BoardSize { get; init; }
    public required PieceTransformation PieceTransformation { get; init; }
    public List<string> PieceIds { get; init; } = [];

    public Config Normalise() => this with
    {
        Id = Id.NormaliseId(),
        PieceIds = PieceIds.Select(id => id.NormaliseId()).ToList()
    };
}
