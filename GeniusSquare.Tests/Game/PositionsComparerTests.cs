using GeniusSquare.Core.Coords;
using GeniusSquare.Core.Game;
using FluentAssertions;

namespace GeniusSquare.Core.Tests.Game;

[TestFixture]
public class PositionsComparerTests
{
    private static IEqualityComparer<OrientedPiece> _positionsComparer = new PositionsComparer<OrientedPiece>();

    [TestCase("C2")]
    [TestCase("A1", "B1", "C1", "C2")]
    public void Equals_WithEqualPositions_IsTrue(params string[] positions)
    {
        OrientedPiece a = CreateOrientedPiece(Orientation.Ar, positions.Select(Coord.Parse));
        OrientedPiece b = CreateOrientedPiece(Orientation.Br, positions.Select(Coord.Parse));

        _positionsComparer.Equals(a, b).Should().BeTrue();
    }

    [TestCase("C2")]
    [TestCase("A1", "B1", "C1", "C2")]
    public void GetHashCode_WithEqualPositions_ReturnsSame(params string[] positions)
    {
        OrientedPiece a = CreateOrientedPiece(Orientation.Ar, positions.Select(Coord.Parse));
        OrientedPiece b = CreateOrientedPiece(Orientation.Br, positions.Select(Coord.Parse));

        int aHashCode = _positionsComparer.GetHashCode(a);
        int bHashCode = _positionsComparer.GetHashCode(b);
        aHashCode.Should().Be(bHashCode);
    }

    [TestCase("C2")]
    [TestCase("A1", "B1", "C1", "C2")]
    public void Distinct_WithEqualPositions_ReturnsSingle(params string[] positions)
    {
        OrientedPiece[] pieces =
        {
            CreateOrientedPiece(Orientation.Ar, positions.Select(Coord.Parse)),
            CreateOrientedPiece(Orientation.Br, positions.Select(Coord.Parse))
        };

        pieces.Distinct(_positionsComparer).Should().HaveCount(1);
    }


    private static OrientedPiece CreateOrientedPiece(Orientation orientation, IEnumerable<Coord> positions) =>
        CreateOrientedPiece(orientation, positions.ToArray());

    private static OrientedPiece CreateOrientedPiece(Orientation orientation, params Coord[] positions) =>
        new(_piece, orientation, positions);

    private static Piece _piece = new("Test");
}
