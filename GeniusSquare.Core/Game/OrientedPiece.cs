﻿using GeniusSquare.Core.Coords;

namespace GeniusSquare.Core.Game;
public sealed class OrientedPiece : IEquatable<OrientedPiece>
{
    public Piece Piece { get; }
    public string Name { get; }

    /// <summary>
    /// The list of occupied positions (normalised for coordinate translation and ordering)
    /// </summary>
    public IReadOnlyList<Coord> Positions { get; }
    public CoordRange Bounds { get; }

    // TODO: Make constructor private with factory method?
    public OrientedPiece(Piece piece, string name, IEnumerable<Coord> positions)
    {
        Piece = piece;
        Name = name;
        Positions = Normalise(positions).ToList();
        Bounds = Positions.GetBounds();
    }

    private static IEnumerable<Coord> Normalise(IEnumerable<Coord> positions)
    {
        List<Coord> positionsList = positions.ToList(); // copy for multiple enumeration
        CoordRange bounds = positionsList.GetBounds();
        return positionsList
            .Transpose(-bounds.Start) // transpose to origin
            .Order(); // apply standard ordering
    }

    /// <remarks>
    /// OrientedPieces are considered equal iff they contain the same normalised positions (regardless of Piece or Name or coordinate translation)
    /// </remarks>
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
