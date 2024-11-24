using GeniusSquare.Core.Coords;
using GeniusSquare.Core.Game;
using FluentAssertions;
using NUnit.Framework;
using System.Text.RegularExpressions;
using FluentAssertions.Equivalency.Tracing;

namespace GeniusSquare.Core.Tests
{
    [TestFixture]
    public class OrientedPieceTests
    {
        [TestCase("(0,0)(0,1)(1,0)", "(0,0)(0,1)(1,0)")]
        [TestCase("(3,0)(3,1)(4,0)","(0,0)(0,1)(1,0)")]
        [TestCase("(0,3)(0,4)(1,3)", "(0,0)(0,1)(1,0)")]
        [TestCase("(-3,0)(-3,1)(-2,0)", "(0,0)(0,1)(1,0)")]
        [TestCase("(0,-3)(0,-2)(1,-3)", "(0,0)(0,1)(1,0)")]
        public void Positions_AreTranslatedToNormal(string inputPositions, string expectedNormalPositions) =>
            Positions_AreExpected(
                ParseCoords(inputPositions),
                ParseCoords(expectedNormalPositions));

        [TestCase("(0,0)(0,1)(1,0)(1,2)", "(0,0)(0,1)(1,0)(1,2)")]
        [TestCase("(0,1)(0,0)(1,2)(1,0)", "(0,0)(0,1)(1,0)(1,2)")]
        [TestCase("(1,2)(1,0)(0,1)(0,0)", "(0,0)(0,1)(1,0)(1,2)")]
        public void Positions_AreReorderedToNormal(string inputPositions, string expectedNormalPositions) =>
            Positions_AreExpected(
                ParseCoords(inputPositions),
                ParseCoords(expectedNormalPositions));

        private void Positions_AreExpected(IEnumerable<Coord> inputPositions, IEnumerable<Coord> expectedPositions)
        {
            OrientedPiece sut = CreateOrientedPiece("Test", inputPositions);
            sut.Positions.Should().BeEquivalentTo(expectedPositions, o => o.WithStrictOrdering());
        }

        [Test]
        public void Equals_WithEqualPositions_IsTrue()
        {
            OrientedPiece a = CreateOrientedPiece("A", new(0, 0), new(0, 1), new(0, 2));
            OrientedPiece b = CreateOrientedPiece("B", new(0, 0), new(0, 1), new(0, 2));

            a.Equals(b).Should().BeTrue();
        }

        [Test]
        public void GetHashCode_WithEqualPositions_ReturnsSame()
        {
            OrientedPiece a = CreateOrientedPiece("A", new(0, 0), new(0, 1), new(0, 2));
            OrientedPiece b = CreateOrientedPiece("B", new(0, 0), new(0, 1), new(0, 2));

            a.GetHashCode().Should().Be(b.GetHashCode());
        }

        [Test]
        public void Distinct_WithEqualPositions_ReturnsSingle()
        {
            OrientedPiece[] pieces =
            {
                CreateOrientedPiece("A", new(0, 0), new(0, 1), new(0, 2)),
                CreateOrientedPiece("B", new(0, 0), new(0, 1), new(0, 2))
            };

            pieces.Distinct().Should().HaveCount(1);
        }

        private static IEnumerable<Coord> ParseCoords(string s)
        {
            // e.g. "(-1,2)(0,2)(1,2)" with named captures (x,y) per coord:
            const string CoordListRegex = @"^(\((?<x>[-+]?\d+),(?<y>[-+]?\d+)\))*$";

            var match = Regex.Match(s, CoordListRegex);
            var xs = match.Groups["x"].Captures.Select(x => x.Value);
            var ys = match.Groups["y"].Captures.Select(y => y.Value);

            Coord ParseCoord(string x, string y) => new Coord(int.Parse(x), int.Parse(y));
            var coords = xs.Zip(ys, ParseCoord).ToList();

            return coords;
        }

        private static OrientedPiece CreateOrientedPiece(string name, IEnumerable<Coord> positions) => CreateOrientedPiece(name, positions.ToArray());
        private static OrientedPiece CreateOrientedPiece(string name, params Coord[] positions) => new OrientedPiece(_piece, name, positions);
        private static Piece _piece = new("Test", ConsoleColor.Black);
    }
}