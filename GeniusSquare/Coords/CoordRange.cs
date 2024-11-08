namespace GeniusSquare.Coords;

/// <summary>
/// Defines a bounding box with half-open coordinate ranges
/// </summary>
/// <param name="Start">Start of bounding coordinate range (inclusive)</param>
/// <param name="End">End of bounding coordinate range (exclusve)</param>
public record struct CoordRange(Coord Start, Coord End)
{
    public IEnumerable<Coord> EnumerateCoords()
    {
        foreach (int y in EnumerateY())
        {
            foreach (int x in EnumerateX())
            {
                yield return new Coord(x, y);
            }
        }
    }

    public IEnumerable<int> EnumerateX() => Range(Start.X, End.X);
    public IEnumerable<int> EnumerateY() => Range(Start.Y, End.Y);
    private static IEnumerable<int> Range(int start, int end) => Enumerable.Range(start, end - start);

    public override string ToString() => $"[{Start},{End}]";
}
