using FluentAssertions;
using GeniusSquare.Core.Coords;

namespace GeniusSquare.Core.Tests.Coords;

[TestFixture]
internal class CoordTests
{
    [TestCase(0, 0, 0, 0, 0, 0)]
    [TestCase(0, 0, 1, 2, 1, 2)]
    [TestCase(1, 2, 0, 0, 1, 2)]
    [TestCase(1, 2, 3, 4, 4, 6)]
    [TestCase(1, 2, -2, -1, -1, 1)]
    public static void OperatorPlus_AddsComponents(int ax, int ay, int bx, int by, int ex, int ey)
    {
        var actual = new Coord(ax, ay) + new Coord(bx, by);
        var expected = new Coord(ex, ey);
        actual.Should().Be(expected);
    }

    [TestCase(0, 0, 0, 0, 0, 0)]
    [TestCase(0, 0, 1, 2, -1, -2)]
    [TestCase(1, 2, 0, 0, 1, 2)]
    [TestCase(4, 6, 3, 4, 1, 2)]
    [TestCase(-1, 2, -2, -1, 1, 3)]
    public static void OperatorMinus_SubtractsComponents(int ax, int ay, int bx, int by, int ex, int ey)
    {
        var actual = new Coord(ax, ay) - new Coord(bx, by);
        var expected = new Coord(ex, ey);
        actual.Should().Be(expected);
    }

    [TestCase(0, 0, 0, 0)]
    [TestCase(1, 2, -1, -2)]
    [TestCase(-3, -4, 3, 4)]
    public static void OperatorMinus_NegatesComponents(int ax, int ay, int ex, int ey)
    {
        var actual = - new Coord(ax, ay);
        var expected = new Coord(ex, ey);
        actual.Should().Be(expected);
    }

    [TestCase(1, 2, 3, 4, 1, 2)]
    [TestCase(4, 3, 2, 1, 2, 1)]
    [TestCase(1, 4, 2, 3, 1, 3)]
    public static void Min_ReturnsMininumComponents(int ax, int ay, int bx, int by, int ex, int ey)
    {
        var actual = Coord.Min(new Coord(ax, ay), new Coord(bx, by));
        var expected = new Coord(ex, ey);
        actual.Should().Be(expected);
    }

    [TestCase(1, 2, 3, 4, 3, 4)]
    [TestCase(4, 3, 2, 1, 4, 3)]
    [TestCase(1, 4, 2, 3, 2, 4)]
    public static void Max_ReturnsMaximumComponents(int ax, int ay, int bx, int by, int ex, int ey)
    {
        var actual = Coord.Max(new Coord(ax, ay), new Coord(bx, by));
        var expected = new Coord(ex, ey);
        actual.Should().Be(expected);
    }

    [TestCase(0, 0, 0, 0, 0)]
    [TestCase(1, 2, 1, 2, 0)]
    [TestCase(1, 1, 1, 2, -1)]
    [TestCase(1, 2, 2, 2, -1)]
    [TestCase(2, 1, 1, 2, -1)]
    [TestCase(1, 2, 3, 4, -1)]
    [TestCase(2, 1, 1, 1, +1)]
    [TestCase(2, 2, 1, 2, +1)]
    [TestCase(1, 2, 2, 1, +1)]
    [TestCase(4, 3, 2, 1, +1)]
    public static void CompareTo_ReturnsComparison(int ax, int ay, int bx, int by, int expected)
    {
        var actual = new Coord(ax, ay).CompareTo(new Coord(bx, by));
        actual.Should().Be(expected);
    }

    [TestCase(0, 0, 0, 0)]
    [TestCase(1, 2, -2, 1)]
    [TestCase(-1, -2, 2, -1)]
    public static void Rotate90_TransformsCoord(int ax, int ay, int ex, int ey)
    {
        var actual = new Coord(ax, ay).Rotate90();
        var expected = new Coord(ex, ey);
        actual.Should().Be(expected);
    }

    [TestCase(0, 0, 0, 0)]
    [TestCase(1, 2, -1, -2)]
    [TestCase(-1, -2, 1, 2)]
    public static void Rotate180_TransformsCoord(int ax, int ay, int ex, int ey)
    {
        var actual = new Coord(ax, ay).Rotate180();
        var expected = new Coord(ex, ey);
        actual.Should().Be(expected);
    }

    [TestCase(0, 0, 0, 0)]
    [TestCase(1, 2, 2, -1)]
    [TestCase(-1, -2, -2, 1)]
    public static void Rotate270_TransformsCoord(int ax, int ay, int ex, int ey)
    {
        var actual = new Coord(ax, ay).Rotate270();
        var expected = new Coord(ex, ey);
        actual.Should().Be(expected);
    }

    [TestCase(0, 0, 0, 0)]
    [TestCase(1, 2, -1, 2)]
    [TestCase(-1, -2, 1, -2)]
    public static void Reflect_TransformsCoord(int ax, int ay, int ex, int ey)
    {
        var actual = new Coord(ax, ay).Reflect();
        var expected = new Coord(ex, ey);
        actual.Should().Be(expected);
    }

    [TestCase(0, 0, 0, 0)]
    [TestCase(1, 2, 2, 1)]
    [TestCase(-1, -2, -2, -1)]
    public static void ReflectRotate90_TransformsCoord(int ax, int ay, int ex, int ey)
    {
        var actual = new Coord(ax, ay).ReflectRotate90();
        var expected = new Coord(ex, ey);
        actual.Should().Be(expected);
    }

    [TestCase(0, 0, 0, 0)]
    [TestCase(1, 2, 1, -2)]
    [TestCase(-1, -2, -1, 2)]
    public static void ReflectRotate180_TransformsCoord(int ax, int ay, int ex, int ey)
    {
        var actual = new Coord(ax, ay).ReflectRotate180();
        var expected = new Coord(ex, ey);
        actual.Should().Be(expected);
    }

    [TestCase(0, 0, 0, 0)]
    [TestCase(1, 2, -2, -1)]
    [TestCase(-1, -2, 2, 1)]
    public static void ReflectRotate270_TransformsCoord(int ax, int ay, int ex, int ey)
    {
        var actual = new Coord(ax, ay).ReflectRotate270();
        var expected = new Coord(ex, ey);
        actual.Should().Be(expected);
    }

    [TestCase("(0,0)..(1,1)", "0,0")]
    [TestCase("(0,0)..(3,3)", "0,0", "1,1", "2,2")]
    [TestCase("(0,0)..(3,3)", "0,2", "1,1", "2,0")]
    [TestCase("(0,0)..(3,3)", "0,1", "1,0", "2,1", "1,2")]
    public static void GetBounds_ReturnsBoundingOpenCoordRange(string range, params string[] coords)
    {
        var actual = coords.Select(Coord.Parse).GetBounds();
        var expected = CoordRange.Parse(range);
        actual.Should().Be(expected); 
    }

    [TestCase("A1", 0, 0)]
    [TestCase("A2", 1, 0)]
    [TestCase("B1", 0, 1)]
    [TestCase("C4", 3, 2)]
    [TestCase("  B1   ", 0, 1)]
    public static void Parse_WithIndex_ReturnsCoord(string coord, int ex, int ey) =>
        Parse_ReturnsCoord(coord, new(ex, ey));

    [TestCase("0,0", 0, 0)]
    [TestCase("1,0", 1, 0)]
    [TestCase("0,1", 0, 1)]
    [TestCase("3,2", 3, 2)]
    [TestCase("-2,+3", -2, 3)]
    [TestCase("  0,  1   ", 0, 1)]
    public static void Parse_WithComponents_ReturnsCoord(string coord, int ex, int ey) =>
        Parse_ReturnsCoord(coord, new(ex, ey));

    [TestCase("(0,0)", 0, 0)]
    [TestCase("(1,0)", 1, 0)]
    [TestCase("(0,1)", 0, 1)]
    [TestCase("(3,2)", 3, 2)]
    [TestCase("(-2,+3)", -2, 3)]
    [TestCase(" (  0,  1   )  ", 0, 1)]
    public static void Parse_WithBracketedComponents_ReturnsCoord(string coord, int ex, int ey) =>
        Parse_ReturnsCoord(coord, new(ex, ey));

    private static void Parse_ReturnsCoord(string coord, Coord expected)
    {
        Coord actual = Coord.Parse(coord);
        actual.Should().Be(expected);

        Coord.TryParse(coord, out actual).Should().BeTrue();
        actual.Should().Be(expected);
    }

    [TestCase("")]
    [TestCase("1")]
    [TestCase("1,2,3")]
    [TestCase("1.0,2.0")]
    [TestCase("(1,2")]
    [TestCase("1,2)")]
    [TestCase("A,1")]
    [TestCase("(A1)")]
    public static void Parse_WithInvalidFormat_ThrowsFormatException(string coord)
    {
        Action parse = () => Coord.Parse(coord);
        parse.Should().Throw<FormatException>();

        Coord.TryParse(coord, out var actual).Should().BeFalse();
        actual.Should().Be(default);
    }
}
