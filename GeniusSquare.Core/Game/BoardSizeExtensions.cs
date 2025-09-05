using GeniusSquare.Core.Coords;

namespace GeniusSquare.Core.Game;

public static class BoardSizeExtensions
{
    // TODO: Refactor: Board is not a BoardSize and it has Placements. Just pass a Coord boardSize to Layout. Drop IBoardSize.
    public static Piece?[,] GetLayout(this IBoardSize board) => board.GetLayout(Enumerable.Empty<Placement>());
    public static Piece?[,] GetLayout(this IBoardSize board, Solution solution) => board.GetLayout(solution.Placements);
    public static Piece?[,] GetLayout(this IBoardSize board, IEnumerable<Placement> placements)
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
