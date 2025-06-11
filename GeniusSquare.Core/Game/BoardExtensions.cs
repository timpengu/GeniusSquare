using GeniusSquare.Core.Coords;
using MoreLinq;

namespace GeniusSquare.Core.Game;

public static class BoardExtensions
{
    /// <summary>
    /// Returns a board with the given number of additional random occupied positions (or fewer if the board is full)
    /// </summary>
    public static Board WithOccupiedRandomPositions(this Board board, int randomPositions) =>
        board.WithOccupiedPositions(
            board.UnoccupiedPositions()
                .Shuffle() // generate random permutation of unoccupied positions
                .Take(randomPositions)); // take the first N (if available)

    /// <summary>
    /// Gets the number of surplus positions that will remain once all the given pieces are placed on the board
    /// </summary>
    /// <returns>The number of surplus positions, which may be positive, zero, or negative if there is insufficient space for the pieces</returns>
    public static int SurplusPositions(this Board board, IEnumerable<Piece> pieces)
    {
        int unoccupiedPositions = board.UnoccupiedPositions().Count();
        int placementPositions = pieces.Sum(p => p.PositionCount);
        return unoccupiedPositions - placementPositions;
    }

    public static IEnumerable<Coord> OccupiedPositions(this Board board) => board.Where(board.IsOccupied);
    public static IEnumerable<Coord> UnoccupiedPositions(this Board board) => board.Where(coord => !board.IsOccupied(coord));
    private static IEnumerable<Coord> Where(this Board board, Func<Coord, bool> predicate) => board.Bounds.EnumerateCoords().Where(predicate);

    public static Piece?[,] GetLayout(this Board board) => board.GetLayout(Enumerable.Empty<Placement>());
    public static Piece?[,] GetLayout(this Board board, Solution solution) => board.GetLayout(solution.Placements);
    public static Piece?[,] GetLayout(this Board board, IEnumerable<Placement> placements)
    {
        Piece?[,] layout = new Piece?[board.XSize, board.YSize];

        foreach (Placement placement in placements)
        {
            foreach (Coord pos in placement.Positions)
            {
                layout[pos.X, pos.Y] = placement.OrientedPiece.Piece;
            }
        }

        return layout;
    }
}
