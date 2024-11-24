using GeniusSquare.Core.Coords;
using GeniusSquare.Core.Game;
using FluentAssertions;

namespace GeniusSquare.Core.Tests
{
    [TestClass]
    public class OrientedPieceTests
    {
        [TestMethod]
        public void Equals_WithEqualPositions_IsTrue()
        {
            OrientedPiece a = CreateOrientedPiece("A", new(0, 0), new(0, 1), new(0, 2));
            OrientedPiece b = CreateOrientedPiece("B", new(0, 0), new(0, 1), new(0, 2));

            a.Equals(b).Should().BeTrue();
        }

        [TestMethod]
        public void GetHashCode_WithEqualPositions_ReturnsSame()
        {
            OrientedPiece a = CreateOrientedPiece("A", new(0, 0), new(0, 1), new(0, 2));
            OrientedPiece b = CreateOrientedPiece("B", new(0, 0), new(0, 1), new(0, 2));

            a.GetHashCode().Should().Be(b.GetHashCode());
        }

        [TestMethod]
        public void Distinct_WithEqualPositions_ReturnsSingle()
        {
            OrientedPiece[] pieces =
            {
                CreateOrientedPiece("A", new(0, 0), new(0, 1), new(0, 2)),
                CreateOrientedPiece("B", new(0, 0), new(0, 1), new(0, 2))
            };

            pieces.Distinct().Should().HaveCount(1);
        }

        private static OrientedPiece CreateOrientedPiece(string name, params Coord[] positions) => new OrientedPiece(_piece, name, positions);
        private static Piece _piece = new("Test", ConsoleColor.Black);
    }
}