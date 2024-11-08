namespace GeniusSquare.Game;

/// <summary>
/// Compares pieces according to a heuristic placement difficulty metric
/// </summary>
public sealed class PlacementDifficultyComparer : IComparer<Piece>
{
    public int Compare(Piece? a, Piece? b)
    {
        if (a == null && b == null) return 0;
        if (a == null) return -1;
        if (b == null) return 1;

        int comparePositions = a.Positions.CompareTo(b.Positions); // more positions are more difficult to place
        if (comparePositions != 0) return comparePositions;

        // TODO: Consider eccentricity? i.e. longest dimension divided by shortest dimension

        int compareOrientations = b.Orientations.Count.CompareTo(a.Orientations.Count); // fewer orientations are more difficult to place
        if (compareOrientations != 0) return compareOrientations;

        return 0; // otherwise similar difficulty
    }
}
