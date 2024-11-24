using GeniusSquare.CommandLine;
using GeniusSquare.Configuration;
using GeniusSquare.Core;
using GeniusSquare.Core.Coords;
using GeniusSquare.Core.Game;
using System.Diagnostics;

namespace GeniusSquare;

internal class ConsoleRunner
{
    private readonly Options _args;

    public ConsoleRunner(Options args)
    {
        _args = args;
    }

    public bool Run()
    {
        Config config = Config.Load("Config.json");

        IList<Piece> pieces = config.GeneratePieces().ToList();
        Board board = _args.GenerateBoard(config, pieces);

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

        Stopwatch sw = new();
        sw.Start();

        int solutionCount = 0;
        foreach (Solution solution in Solver.GetSolutions(board, pieces))
        {
            ++solutionCount;

            if (_args.HasVerboseSolutions())
            {
                Console.WriteLine($"\nSolution {solutionCount} @ {sw.Elapsed:hh\\:mm\\:ss\\.fff}");
                ConsoleWriteColouredLayout(board, pieces, solution);
            }

            if (_args.HasVerbosePlacements())
            {
                Console.WriteLine(String.Join('\n', solution.Placements));
            }
        }

        sw.Stop();

        Console.WriteLine($"\nFound {solutionCount} solution{(solutionCount == 1 ? "" : "s")} in {sw.Elapsed:hh\\:mm\\:ss\\.fff}.");

        return solutionCount > 0;
    }

    private static void ConsoleWriteColouredLayout(Board board, IList<Piece> pieces, Solution solution)
    {
        Piece?[,] layout = new Piece?[board.XSize, board.YSize];

        // TODO: Add back ref to Piece from OrientedPiece to access Piece properties for Placements
        // HACK: Construct dictionary lookup of Piece per OrientedPiece
        Dictionary<OrientedPiece, Piece> pieceDictionary = pieces
            .SelectMany(
                p => p.Orientations,
                (p, op) => (Piece: p, OrientedPiece: op))
            .ToDictionary(
                p => p.OrientedPiece,
                p => p.Piece);

        foreach (Placement placement in solution.Placements)
        {
            foreach (Coord pos in placement.Positions)
            {
                layout[pos.X, pos.Y] = pieceDictionary[placement.OrientedPiece];
            }
        }

        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.Black;

        foreach (int y in board.Bounds.EnumerateY())
        {
            foreach (int x in board.Bounds.EnumerateX())
            {
                Piece? piece = layout[x, y];

                Console.BackgroundColor = piece?.ConsoleColor ?? ConsoleColor.Black;
                Console.Write(piece?.Name ?? "..");
            }

            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine();
        }
    }
}
