using GeniusSquare.Core.Game;

namespace GeniusSquare.Core;

public static class Solver
{
    public static IEnumerable<Solution> GetSolutions(Board board, IEnumerable<Piece> pieces)
    {
        // Sort pieces heuristically by descending difficulty of placement
        var orderedPieces = pieces
            .OrderDescending(new PlacementDifficultyComparer())
            .ToList(); 

        return board.Place(orderedPieces);
    }

    private static IEnumerable<Solution> Place(this Board board, IEnumerable<Piece> pieces)
    {
        if (!pieces.Any())
        {
            // No more pieces to place, return the placements as a single solution
            return [new Solution(board.Placements)];
        }

        return board
            .GetPlacements(pieces.First()) // get first piece placements
            .SelectMany(placement =>
                board
                    .WithPlacement(placement) // add placement to board
                    .Place(pieces.Skip(1)) // place subsequent pieces recursively
            )
            .ToList();
    }

    private static IEnumerable<Placement> GetPlacements(this Board board, Piece piece)
        => piece
            .Orientations
            .SelectMany(board.GetPlacements);

    private static IEnumerable<Placement> GetPlacements(this Board board, OrientedPiece orientedPiece)
        => orientedPiece
            .GetPlacementRange(board) // get coord range of possible placement positions
            .EnumerateCoords()
            .Select(coord => new Placement(orientedPiece, coord))
            .Where(placement => !board.IsOccupied(placement)); // exclude occupied positions
}
