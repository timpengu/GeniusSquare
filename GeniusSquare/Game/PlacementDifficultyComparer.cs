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

        int aPositions = a.Orientations[0].Positions.Count;
        int bPositions = b.Orientations[0].Positions.Count;
        int comparePositions = aPositions.CompareTo(bPositions); // more positions are more difficult to place
        if (comparePositions != 0) return comparePositions;

        // TODO: Consider eccentricity? i.e. longest dimension divided by shortest dimension

        int aOrientations = a.Orientations.Count;
        int bOrientations = b.Orientations.Count;
        int compareOrientations = -aOrientations.CompareTo(bOrientations); // fewer orientations are more difficult to place
        if (compareOrientations != 0) return compareOrientations;

        return 0; // otherwise similar difficulty
    }
}
