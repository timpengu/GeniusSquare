using GeniusSquare.Configuration;
using GeniusSquare.Core.Coords;
using GeniusSquare.Core.Game;
using MoreLinq;

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
            // Get board size from args/cofnig
            Coord boardSize = opts.GetBoardSize()
                ?? config.GetDefaultBoardSize()
                ?? throw new OptionsException($"Missing {nameof(Options.BoardSize)} option and {nameof(Config.DefaultBoardSize)} config.");

            // Get distinct occupied positions from args
            ISet<Coord> occupiedPositions = opts.OccupiedPositions.Select(Coord.Parse).ToHashSet();

            // Get number of remaining positions to occupy randomly
            int piecePositions = pieces.Sum(p => p.Positions);
            int remainingPositions = boardSize.X * boardSize.Y - occupiedPositions.Count - piecePositions; // (can be <= 0)
            int randomPositions = opts.OccupiedRandoms ?? remainingPositions; // occupy all remaining positions by default

            // Create board with specified occupied positions
            var board = Board
                .Create(boardSize)
                .WithOccupiedPositions(occupiedPositions);

            // Add random occupied positions to board
            board = board.WithOccupiedPositions(
                board.Bounds
                    .EnumerateCoords()
                    .Where(coord => !board.IsOccupied(coord))
                    .Shuffle() // generate a random permutation of unoccupied positions
                    .Take(randomPositions)); // take the first N (if available)

            return board;
        }
    }
}
