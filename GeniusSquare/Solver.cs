using System.Collections.Generic;
using GeniusSquare.Game;

namespace GeniusSquare;

public static class Solver
{
    public static IEnumerable<Solution> PlaceAll(this Board board, IEnumerable<Piece> pieces)
    {
        // Sort pieces heuristically by descending difficulty of placement
        var orderedPieces = pieces
            .OrderDescending(new PlacementDifficultyComparer())
            .ToList(); 

        return board.PlaceAll(
            orderedPieces,
            Enumerable.Empty<Placement>() // start with no placements
        );
    }

    // TOOD: move placements param into board state (and show in ToString)
    public static IEnumerable<Solution> PlaceAll(this Board board, IEnumerable<Piece> pieces, IEnumerable<Placement> placed)
    {
        if (pieces.Any())
        {
            // Find all placements for the first piece
            Piece piece = pieces.First();
            List<Placement> placements = piece.GetPlacements(board).ToList();

            return placements
                .SelectMany(placement =>
                    board
                        .WithPlacement(placement) // add placement to the board
                        .PlaceAll( // place subsequent pieces recursively
                            // TODO: enumerate sequences into collections to avoid stacking functors?
                            pieces.Skip(1), 
                            placed.Append(placement)
                ))
                .ToList();
        }
        else
        {
            // No more pieces remain to be placed, return the placements as a unique solution
            return Enumerable.Repeat(new Solution(placed.ToList()), 1);
        }
    }
}
