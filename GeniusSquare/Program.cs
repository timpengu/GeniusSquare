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

        Console.WriteLine("Initial board state:");
        Console.WriteLine(board);
        Console.WriteLine("Placing pieces (with orientations):");
        foreach (Piece piece in pieces)
        {
            Console.WriteLine($"{piece} ({String.Join(",", piece.Orientations.Select(op => op.Name))})");
        }
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
