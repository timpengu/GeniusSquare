using GeniusSquare.Coords;

namespace GeniusSquare.Game;
public sealed record OrientedPiece : IEquatable<OrientedPiece>
{
    public string Name { get; }
    public IReadOnlyList<Coord> Positions { get; }
    public CoordRange Bounds { get; }

    public OrientedPiece(string name, IEnumerable<Coord> positions)
    {
        Name = name;
        Positions = Normalise(positions).ToList();
        Bounds = Positions.GetBounds();
    }

    // TODO: Allow denormalised instances and let caller normalise positions?
    private static IEnumerable<Coord> Normalise(IEnumerable<Coord> positions)
    {
        List<Coord> positionsList = positions.ToList(); // copy for multiple enumeration
        CoordRange bounds = positionsList.GetBounds();
        return positionsList
            .Transpose(-bounds.Start) // transpose to origin
            .Order(); // apply standard ordering
    }

    public bool Equals(OrientedPiece? other)
    {
        if (ReferenceEquals(other, null)) return false;
        if (ReferenceEquals(other, this)) return true;
        
        return Positions.SequenceEqual(other.Positions);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return Positions.Aggregate(19, (hashCode, position) => 31 * hashCode + position.GetHashCode());
        }
    }

    public override string ToString() => $"{Name}[{string.Join(",", Positions)}]";
}
