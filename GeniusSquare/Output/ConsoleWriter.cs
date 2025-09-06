using GeniusSquare.CommandLine;
using GeniusSquare.Core.Game;

namespace GeniusSquare.Output;

internal class ConsoleWriter : IOutputWriter
{
    private readonly bool _writeSolutions;
    private readonly bool _writePlacements;

    public ConsoleWriter(Options options)
    {
        _writeSolutions = options.HasVerboseSolutions();
        _writePlacements = options.HasVerbosePlacements();
    }

    public void WriteInitialState(Board board, IReadOnlyCollection<Piece> pieces)
    {
        int boardPositions = board.Bounds.EnumerateCoords().Count(coord => !board.IsOccupied(coord));
        int piecePositions = pieces.Sum(p => p.PositionCount);

        Console.WriteLine("Initial board state:");
        Console.WriteLine(board);
        Console.WriteLine();

        Console.WriteLine("Placing pieces (with orientations):");
        foreach (Piece piece in pieces)
        {
            Console.WriteLine($"{piece} ({String.Join(",", piece.Orientations.Select(o => o.Orientation))})");
        }
        Console.WriteLine();

        Console.WriteLine($"Board positions: {boardPositions} unoccupied");
        Console.WriteLine($"Piece positions: {piecePositions} to place");
        string constraint = piecePositions.CompareTo(boardPositions) switch
        {
            < 0 => "under",
            > 0 => "over",
            _ => "critically",
        };
        Console.WriteLine($"Board is {constraint} constrained.");
    }

    public void WriteSolution(Board board, Solution solution, int solutionCount, TimeSpan elapsed)
    {
        if (_writeSolutions)
        {
            Console.WriteLine($"\nSolution {solutionCount} @ {elapsed:hh\\:mm\\:ss\\.fff}");
            ConsoleWriteColouredLayout(board, solution);
        }

        if (_writePlacements)
        {
            Console.WriteLine(string.Join('\n', solution.Placements));
        }
    }

    public void WriteSummary(int solutionCount, TimeSpan elapsed)
    {
        Console.WriteLine($"\nFound {solutionCount} solution{(solutionCount == 1 ? "" : "s")} in {elapsed:hh\\:mm\\:ss\\.fff}.");
    }

    private void ConsoleWriteColouredLayout(Board board, Solution solution)
    {
        Piece?[,] pieces = solution.GetLayout(board.Size);

        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.Black;

        foreach (int y in board.Bounds.EnumerateY())
        {
            foreach (int x in board.Bounds.EnumerateX())
            {
                Piece? piece = pieces[x, y];

                Console.BackgroundColor = piece?.Attributes.ConsoleColor ?? ConsoleColor.Black;
                Console.Write(piece?.Name ?? "..");
            }

            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine();
        }
    }

    public void Flush()
    {
    }
}
