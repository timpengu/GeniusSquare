using GeniusSquare.Core.Coords;
using GeniusSquare.Core.Game;
using FluentAssertions;

namespace GeniusSquare.Core.Tests.Game;

[TestFixture]
public class OrientedPieceTests
{
    [TestCase(new[] { "A1", "A2", "B1" }, new[] { "A1", "A2", "B1" })]
    [TestCase(new[] { "A2", "A3", "B2" }, new[] { "A1", "A2", "B1" })]
    [TestCase(new[] { "C1", "C2", "D1" }, new[] { "A1", "A2", "B1" })]
    [TestCase(new[] { "(-3,0)", "(-3,1)", "(-2,0)" }, new[] { "(0,0)", "(0,1)", "(1,0)" })]
    [TestCase(new[] { "(0,-3)", "(0,-2)", "(1,-3)" }, new[] { "(0,0)", "(0,1)", "(1,0)" })]
    public void Positions_AreTranslatedToNormal(string[] inputPositions, string[] normalPositions)
    {
        OrientedPiece sut = CreateOrientedPiece("Test", Orientation.Ar, inputPositions.Select(Coord.Parse));
        sut.Positions.Should().BeEquivalentTo(normalPositions.Select(Coord.Parse));
    }

    [TestCase(new[] { "A1", "B1", "A2", "C2" }, new[] { "A1", "B1", "A2", "C2" })]
    [TestCase(new[] { "B1", "A1", "C2", "A2" }, new[] { "A1", "B1", "A2", "C2" })]
    [TestCase(new[] { "C2", "A2", "B1", "A1" }, new[] { "A1", "B1", "A2", "C2" })]
    public void Positions_AreReorderedToNormal(string[] inputPositions, string[] normalPositions)
    {
        OrientedPiece sut = CreateOrientedPiece("Test", Orientation.Ar, inputPositions.Select(Coord.Parse));
        sut.Positions.Should().BeEquivalentTo(normalPositions.Select(Coord.Parse), o => o.WithStrictOrdering());
    }

    [TestCase("C2")]
    [TestCase("A1", "B1", "C1", "C2")]
    public void Equals_WithEqualPositions_IsTrue(params string[] positions)
    {
        OrientedPiece a = CreateOrientedPiece("A", Orientation.Ar, positions.Select(Coord.Parse));
        OrientedPiece b = CreateOrientedPiece("B", Orientation.Br, positions.Select(Coord.Parse));

        a.Equals(b).Should().BeTrue();
    }

    [TestCase("C2")]
    [TestCase("A1", "B1", "C1", "C2")]
    public void GetHashCode_WithEqualPositions_ReturnsSame(params string[] positions)
    {
        OrientedPiece a = CreateOrientedPiece("A", Orientation.Ar, positions.Select(Coord.Parse));
        OrientedPiece b = CreateOrientedPiece("B", Orientation.Br, positions.Select(Coord.Parse));

        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [TestCase("C2")]
    [TestCase("A1", "B1", "C1", "C2")]
    public void Distinct_WithEqualPositions_ReturnsSingle(params string[] positions)
    {
        OrientedPiece[] pieces =
        {
            CreateOrientedPiece("A", Orientation.Ar, positions.Select(Coord.Parse)),
            CreateOrientedPiece("B", Orientation.Br, positions.Select(Coord.Parse))
        };

        pieces.Distinct().Should().HaveCount(1);
    }

    private static OrientedPiece CreateOrientedPiece(string name, Orientation orientation, IEnumerable<Coord> positions) =>
        CreateOrientedPiece(name, orientation, positions.ToArray());

    private static OrientedPiece CreateOrientedPiece(string name, Orientation orientation, params Coord[] positions) =>
        new(_piece, orientation, name, positions);
    
    private static Piece _piece = new("Test", ConsoleColor.Black);
}