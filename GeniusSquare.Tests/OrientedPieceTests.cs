using GeniusSquare.Coords;
using GeniusSquare.Game;
using FluentAssertions;

namespace GeniusSquare.Tests
{
    [TestClass]
    public class OrientedPieceTests
    {
        [TestMethod]
        public void Equals_WithEqualPositions_IsTrue()
        {
            var a = new OrientedPiece("A", [new(0, 0), new(0, 1), new(0, 2)]);
            var b = new OrientedPiece("B", [new(0, 0), new(0, 1), new(0, 2)]);

            a.Equals(b).Should().BeTrue();
        }

        [TestMethod]
        public void GetHashCode_WithEqualPositions_ReturnsSame()
        {
            var a = new OrientedPiece("A", [new(0, 0), new(0, 1), new(0, 2)]);
            var b = new OrientedPiece("B", [new(0, 0), new(0, 1), new(0, 2)]);

            a.GetHashCode().Should().Be(b.GetHashCode());
        }

        [TestMethod]
        public void Distinct_WithEqualPositions_ReturnsSingle()
        {
            var a = new OrientedPiece("A", [new(0, 0), new(0, 1), new(0, 2)]);
            var b = new OrientedPiece("B", [new(0, 0), new(0, 1), new(0, 2)]);

            OrientedPiece[] pieces = { a, b };
            pieces.Distinct().Should().HaveCount(1);
        }
    }
}