using GeniusSquare.Config;
using GeniusSquare.Game;
using System.Diagnostics;

namespace GeniusSquare;
public class Program
{
    public static void Main()
    {
        ConfigPieces config = ConfigPieces.Load("Pieces.json");
        IEnumerable<Piece> pieces = config.GeneratePieces().ToList();

        Board board = Board
            .Empty(6, 6)
            .WithPositions("A6", "B1", "B5", "D1", "E3", "F2", "F4");

        Console.WriteLine($"Initial board state:\n{board}");

        int solutionCount = 0;

        Stopwatch sw = new();
        sw.Start();

        foreach (Solution solution in board.PlaceAll(pieces))
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
