using GeniusSquare.Core.Game;

namespace GeniusSquare.Output;

internal interface IOutputWriter
{
    void WriteInitialState(Board board, IReadOnlyCollection<Piece> pieces);
    void WriteSolution(Board board, Solution solution, int solutionCount, TimeSpan elapsed);
    void WriteSummary(int solutionCount, TimeSpan elapsed);
    void Flush();
}
