using FluentAssertions;
using GeniusSquare.Core.Coords;
using GeniusSquare.Core.Game;

namespace GeniusSquare.Core.Tests.Game;

[TestFixture]
internal static class BoardTests
{
    [TestCase(0, 0)]
    [TestCase(0, 2)]
    [TestCase(2, 0)]
    [TestCase(3, 2)]
    public static void XSize_ReturnsBoardXDimension(int xSize, int ySize)
    {
        Board.Create(new(xSize, ySize)).XSize.Should().Be(xSize);
    }

    [TestCase(0, 0)]
    [TestCase(0, 2)]
    [TestCase(2, 0)]
    [TestCase(3, 2)]
    public static void YSize_ReturnsBoardYDimension(int xSize, int ySize)
    {
        Board.Create(new(xSize, ySize)).YSize.Should().Be(ySize);
    }

    [TestCase(0, 0)]
    [TestCase(0, 2)]
    [TestCase(2, 0)]
    [TestCase(3, 2)]
    public static void Bounds_ReturnsBoardBounds(int xSize, int ySize)
    {
        var expected = new CoordRange(new(0, 0), new(xSize, ySize));
        Board.Create(new(xSize, ySize)).Bounds.Should().Be(expected);
    }

    [TestCase(0, 0)]
    [TestCase(3, 2)]
    public static void Placements_IsInitiallyEmpty(int xSize, int ySize)
    {
        Board.Create(new(xSize, ySize)).Placements.Should().BeEmpty();
    }

    [TestCase(3, 2)]
    public static void IsOccupied_OverBoardRange_IsInitiallyFalse(int xSize, int ySize)
    {
        var board = Board.Create(new(xSize, ySize));
        board.Bounds.EnumerateCoords().Should().AllSatisfy(coord =>
            board.IsOccupied(coord).Should().BeFalse());
    }

    [TestCase(3, 2, "A1", "B3")]
    public static void WithOccupiedPositions_UpdatesIsOccupied(int xSize, int ySize, params string[] positionIndexes)
    {
        ISet<Coord> occupiedPositions = positionIndexes.Select(Coord.Parse).ToHashSet();

        var board = Board.Create(new(xSize, ySize)).WithOccupiedPositions(occupiedPositions);
        
        board.Bounds.EnumerateCoords().Should().AllSatisfy(coord =>
            board.IsOccupied(coord).Should().Be(occupiedPositions.Contains(coord)));
    }

    [TestCase(3, 2, "A1", "A1", "A2", "B1")]
    [TestCase(3, 2, "A2", "A1", "A2", "B1")]
    public static void WithPlacement_UpdatesPlacementsAndIsOccupied(int xSize, int ySize, string placementIndex, params string[] pieceIndexes)
    {
        var board = Board.Create(new(xSize, ySize));

        var orientedPiece = CreateOrientedPiece(pieceIndexes.Select(Coord.Parse));
        var offset = Coord.Parse(placementIndex);
        var placement = new Placement(orientedPiece, offset);

        board = board.WithPlacement(placement);

        board.Placements.Should().BeEquivalentTo(new[] { placement });
        board.Bounds.EnumerateCoords().Should().AllSatisfy(coord =>
            board.IsOccupied(coord).Should().Be(
                placement.Positions.Contains(coord)));
    }

    private static OrientedPiece CreateOrientedPiece(IEnumerable<Coord> positions) => new(_piece, Orientation.Ar, positions);
    private static Piece _piece = new("Test", ConsoleColor.Black);
}
