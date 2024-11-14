using CommandLine;
using CommandLine.Text;
using GeniusSquare.CommandLine;
using GeniusSquare.Configuration;
using GeniusSquare.Core;
using GeniusSquare.Core.Game;
using System.Diagnostics;

namespace GeniusSquare;
public class Program
{
    public static int Main(string[] args)
    {
        try
        {
            bool success = false;

            ParserResult<Args> parserResult = Parse(args);
            parserResult
                .WithParsed(args =>
                {
                    args.Validate();
                    success = Run(args);
                })
                .WithNotParsed(errors =>
                    throw new ArgsException(GetHelpText(parserResult, errors))
                );

            return success ? 0 : 1;
        }
        catch (ArgsException e)
        {
            Console.WriteLine(e.Message);
            return -1;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return -2;
        }
    }

    private static bool Run(Args args)
    {
        Config config = Config.Load("Config.json");

        IList<Piece> pieces = config.GeneratePieces().ToList();
        Board board = args.GenerateBoard(config, pieces);

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

            if (args.Verbose)
            {
                Console.WriteLine($"Solution {solutionCount} @ {sw.Elapsed:hh\\:mm\\:ss\\.fff}:");
                Console.WriteLine(String.Join("\n", solution.Placements));
                Console.WriteLine();
            }
        }

        sw.Stop();

        Console.WriteLine($"Found {solutionCount} solution{(solutionCount==1?"":"s")} in {sw.Elapsed:hh\\:mm\\:ss\\.fff}.");

        return solutionCount > 0;
    }

    private static ParserResult<Args> Parse(string[] args)
    {
        using var parser = new Parser(with => with.HelpWriter = null);
        return parser.ParseArguments<Args>(args);
    }

    private static string GetHelpText<T>(ParserResult<T> result, IEnumerable<Error> errs)
    {
        return errs.IsVersion()
            ? HelpText.AutoBuild(result)
            : HelpText.AutoBuild(result, h =>
                {
                    h.AdditionalNewLineAfterOption = false;
                    return HelpText.DefaultParsingErrorsHandler(result, h);
                },
                e => e
            );
    }
}
