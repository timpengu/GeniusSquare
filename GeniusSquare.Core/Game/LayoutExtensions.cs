using GeniusSquare.Core.Coords;

namespace GeniusSquare.Core.Game;

// TODO: Use non-extension static class?
public static class LayoutExtensions
{
    public static Piece?[,] GetLayout(this Board board) => board.Placements.GetLayout(board.Size);

    // TODO: reference BoardSize from Solution?
    public static Piece?[,] GetLayout(this Solution solution, Coord boardSize) => solution.Placements.GetLayout(boardSize);

    private static Piece?[,] GetLayout(this IEnumerable<Placement> placements, Coord boardSize)
    {
        Piece?[,] layout = new Piece?[boardSize.X, boardSize.Y];

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
