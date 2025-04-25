using GeniusSquare.CommandLine;
using GeniusSquare.Configuration;
using GeniusSquare.Core;
using GeniusSquare.Core.Game;
using GeniusSquare.Output;
using System.Diagnostics;

namespace GeniusSquare;

internal class Runner
{
    private readonly Config _config;
    private readonly Options _options;

    private readonly List<IOutputWriter> _outputWriters = new();

    public Runner(Config config, Options options)
    {
        _config = config;
        _options = options;

        // TOOD: Add dependency injection?
        _outputWriters.Add(new ConsoleWriter(options));

        if (!String.IsNullOrEmpty(options.HtmlFileName))
        {
            _outputWriters.Add(new HtmlWriter(options.HtmlFileName));
        }
    }

    public bool Run()
    {
        IReadOnlyCollection<Piece> pieces = _config.GeneratePieces().ToList();
        Board board = _options.GenerateBoard(_config, pieces);

        Outputs(o => o.WriteInitialState(board, pieces));

        Stopwatch sw = new();
        sw.Start();

        int solutionCount = 0;
        foreach (Solution solution in Solver.GetSolutions(board, pieces))
        {
            ++solutionCount;

            Outputs(o => o.WriteSolution(board, solution, solutionCount, sw.Elapsed));
        }

        sw.Stop();

        Outputs(o => o.WriteSummary(solutionCount, sw.Elapsed));
        Outputs(o => o.Flush());

        return solutionCount > 0;
    }

    private void Outputs(Action<IOutputWriter> outputAction)
    {
        foreach (var writer in _outputWriters)
        {
            outputAction(writer);
        }
    }
}
