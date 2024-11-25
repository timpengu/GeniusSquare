using GeniusSquare.CommandLine;
using GeniusSquare.Configuration;
using GeniusSquare.Core;
using GeniusSquare.Core.Coords;
using GeniusSquare.Core.Game;
using System.Diagnostics;

namespace GeniusSquare;

internal class ConsoleRunner
{
    private readonly Config _config;
    private readonly Options _options;

    public ConsoleRunner(Config config, Options options)
    {
        _config = config;
        _options = options;
    }

    public bool Run()
    {
        IReadOnlyCollection<Piece> pieces = _config.GeneratePieces().ToList();
        Board board = _options.GenerateBoard(_config, pieces);

        ConsoleWriteInitialState(board, pieces);

        Stopwatch sw = new();
        sw.Start();

        int solutionCount = 0;
        foreach (Solution solution in Solver.GetSolutions(board, pieces))
        {
            ++solutionCount;

            ConsoleWriteSolution(board, solution, solutionCount, sw.Elapsed);
        }

        sw.Stop();

        ConsoleWriteSolutionsSummary(solutionCount, sw.Elapsed);

        return solutionCount > 0;
    }

    private void ConsoleWriteInitialState(Board board, IReadOnlyCollection<Piece> pieces)
    {
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
            < 0 => "under",
            > 0 => "over",
            _ => "critically",
        };
        Console.WriteLine($"Board is {constraint} constrained.");
    }

    private void ConsoleWriteSolution(Board board, Solution solution, int solutionCount, TimeSpan elapsed)
    {
        if (_options.HasVerboseSolutions())
        {
            Console.WriteLine($"\nSolution {solutionCount} @ {elapsed:hh\\:mm\\:ss\\.fff}");
            ConsoleWriteColouredLayout(board, solution);
        }

        if (_options.HasVerbosePlacements())
        {
            Console.WriteLine(String.Join('\n', solution.Placements));
        }
    }

    private void ConsoleWriteSolutionsSummary(int solutionCount, TimeSpan elapsed)
    {
        Console.WriteLine($"\nFound {solutionCount} solution{(solutionCount == 1 ? "" : "s")} in {elapsed:hh\\:mm\\:ss\\.fff}.");
    }

    private void ConsoleWriteColouredLayout(Board board, Solution solution)
    {
        Piece?[,] pieces = new Piece?[board.XSize, board.YSize];

        foreach (Placement placement in solution.Placements)
        {
            foreach (Coord pos in placement.Positions)
            {
                pieces[pos.X, pos.Y] = placement.OrientedPiece.Piece;
            }
        }

        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.Black;

        foreach (int y in board.Bounds.EnumerateY())
        {
            foreach (int x in board.Bounds.EnumerateX())
            {
                Piece? piece = pieces[x, y];

                Console.BackgroundColor = piece?.ConsoleColor ?? ConsoleColor.Black;
                Console.Write(piece?.Name ?? "..");
            }

            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine();
        }
    }
}
