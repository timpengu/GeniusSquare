using GeniusSquare.Configuration;
using GeniusSquare.Core.Coords;
using GeniusSquare.Core.Game;

namespace GeniusSquare.CommandLine
{
    internal static class OptionsExtensions
    {
        public static Options Validate(this Options opts)
        {
            if (opts.BoardSize.Count() > 2)
                throw new OptionsException($"{nameof(opts.BoardSize)} cannot have more than two dimensions.");

            if (opts.BoardSize.Any(x => x < 0))
                throw new OptionsException($"{nameof(opts.BoardSize)} cannot have negative dimensions.");

            if (opts.OccupiedRandoms.HasValue && opts.OccupiedRandoms < 0)
                throw new OptionsException($"{nameof(Options.OccupiedRandoms)} must be positive.");

            if (opts.HtmlFileName != null && opts.HtmlFileName.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                throw new OptionsException($"{nameof(Options.HtmlFileName)} must be a valid file path.");

            if (opts.Verbosity < 0)
                throw new OptionsException($"{nameof(Options.Verbosity)} cannot be negative.");

            return opts;
        }

        public static bool HasVerboseSolutions(this Options opts) => opts.Verbosity >= 1;
        public static bool HasVerbosePlacements(this Options opts) => opts.Verbosity >= 2;

        public static Coord? GetBoardSize(this Options opts)
        {
            List<int> boardSize = opts.BoardSize.Take(2).ToList();
            return boardSize.Count() switch
            {
                1 => new Coord(boardSize[0], boardSize[0]),
                2 => new Coord(boardSize[0], boardSize[1]),
                _ => (Coord?) null
            };
        }

        public static Board GenerateBoard(this Options opts, Config config, IEnumerable<Piece> pieces)
        {
            // Create board with specific/default size
            Board board = Board.Create(
                opts.GetBoardSize() ??
                config.GetDefaultBoardSize() ??
                throw new OptionsException($"Missing {nameof(Options.BoardSize)} option and {nameof(Config.DefaultBoardSize)} config."));

            // Add specific occupied positions
            board = board.WithOccupiedPositions(
                opts.OccupiedPositions.Select(Coord.Parse));

            // Add random occupied positions
            board = board.WithOccupiedRandomPositions(
                opts.OccupiedRandoms ?? // add specified number of random occupied positions, or default to..
                board.CountSurplusPositions(pieces) // critically constrain the board (leave zero unoccupied positions when all pieces are placed)
            );

            return board;
        }
    }
}
