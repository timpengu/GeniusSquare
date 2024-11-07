namespace GeniusSquare.Coords;

/// <summary>
/// Defines a bounding box with half-open coordinate ranges
/// </summary>
/// <param name="Start">Start of bounding coordinate range (inclusive)</param>
/// <param name="End">End of bounding coordinate range (exclusve)</param>
public record struct CoordRange(Coord Start, Coord End)
{
    public override string ToString() => $"[{Start},{End}]";
}
