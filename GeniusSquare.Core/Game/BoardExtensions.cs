using GeniusSquare.Core.Coords;

namespace GeniusSquare.Core.Game;

public static class BoardExtensions
{
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
