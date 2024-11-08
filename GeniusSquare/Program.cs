using GeniusSquare.Configuration;
using GeniusSquare.Game;
using System.Diagnostics;

namespace GeniusSquare;
public class Program
{
    public static void Main()
    {
        Config config = Config.Load("Config.json");

        Board board = config.GenerateBoard();
        IList<Piece> pieces = config.GeneratePieces().ToList();

        int boardPositions = board.Bounds.EnumerateCoords().Count(coord => !board.IsOccupied(coord));
        int piecePositions = pieces.Sum(p => p.Positions);

        Console.WriteLine("Initial board state:");
        Console.WriteLine(board);
        Console.WriteLine();

        Console.WriteLine("Placing pieces (with orientations):");
        foreach (Piece piece in pieces)
        {
            Console.WriteLine($"{piece} ({String.Join(",", piece.Orientations.Select(op => op.Name))})");
        }
        Console.WriteLine();

        Console.WriteLine($"Board positions: {boardPositions} unoccupied");
        Console.WriteLine($"Piece positions: {piecePositions} to place");
        string constraint = piecePositions.CompareTo(boardPositions) switch
        {
            <0 => "under",
            >0 => "over",
            _ => "critically",
        };
        Console.WriteLine($"Board is {constraint} constrained.");
        Console.WriteLine();

        Stopwatch sw = new();
        sw.Start();

        int solutionCount = 0;
        foreach (Solution solution in Solver.GetSolutions(board, pieces))
        {
            ++solutionCount;

            Console.WriteLine($"Solution {solutionCount} @ {sw.Elapsed:hh\\:mm\\:ss\\.fff}:");
            Console.WriteLine(String.Join("\n", solution.Placements));
            Console.WriteLine();
        }

        sw.Stop();

        Console.WriteLine($"Found {solutionCount} solution{(solutionCount==1?"":"s")} in {sw.Elapsed:hh\\:mm\\:ss\\.fff}.");
    }
}
