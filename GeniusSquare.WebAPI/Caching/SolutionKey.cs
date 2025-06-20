using GeniusSquare.Core.Coords;
using MoreLinq;

namespace GeniusSquare.WebAPI.Caching;

public record struct SolutionKey(string configId, IEnumerable<Coord> occupiedPositions) : IEquatable<SolutionKey>
{
    string ConfigId { get; } = configId;
    IReadOnlyCollection<Coord> OccupiedPositions { get; } = occupiedPositions.OrderBy(c => c).Distinct().ToList();

    public bool Equals(SolutionKey other)
    {
        return
            ConfigId.Equals(other.ConfigId) &&
            OccupiedPositions.SequenceEqual(other.OccupiedPositions);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return OccupiedPositions
                .Select(coord => coord.GetHashCode())
                .Prepend(ConfigId.GetHashCode())
                .Aggregate(19, (hashCode, value) => 31 * hashCode + value);
        }
    }

    public override string ToString() => $"{ConfigId},{String.Join(',', OccupiedPositions)}";
}
