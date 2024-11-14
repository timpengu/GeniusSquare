using GeniusSquare.Configuration;
using GeniusSquare.Core.Coords;
using GeniusSquare.Core.Game;
using MoreLinq;

namespace GeniusSquare.CommandLine
{
    internal static class ArgsExtensions
    {
        public static Args Validate(this Args args)
        {
            if (args.BoardSize.Count() > 2)
                throw new ArgsException($"{nameof(args.BoardSize)} cannot have more than two dimensions.");

            if (args.BoardSize.Any(x => x < 0))
                throw new ArgsException($"{nameof(args.BoardSize)} cannot have negative dimensions.");

            if (args.OccupiedRandoms.HasValue && args.OccupiedRandoms < 0)
                throw new ArgsException($"{nameof(Args.OccupiedRandoms)} must be positive.");

            return args;
        }

        public static Coord? GetBoardSize(this Args args)
        {
            List<int> boardSize = args.BoardSize.Take(2).ToList();
            return boardSize.Count() switch
            {
                1 => new Coord(boardSize[0], boardSize[0]),
                2 => new Coord(boardSize[0], boardSize[1]),
                _ => (Coord?) null
            };
        }

        public static Board GenerateBoard(this Args args, Config config, IEnumerable<Piece> pieces)
        {
            // Get board size from args/cofnig
            Coord boardSize = args.GetBoardSize()
                ?? config.GetDefaultBoardSize()
                ?? throw new ArgsException($"Missing {nameof(Args.BoardSize)} option and {nameof(Config.DefaultBoardSize)} config.");

            // Get distinct occupied positions from args
            ISet<Coord> occupiedPositions = args.OccupiedIndexes.ToCoords().ToHashSet();

            // Get number of remaining positions to occupy randomly
            int piecePositions = pieces.Sum(p => p.Positions);
            int remainingPositions = boardSize.X * boardSize.Y - occupiedPositions.Count - piecePositions; // (can be <= 0)
            int randomPositions = args.OccupiedRandoms ?? remainingPositions; // occupy all remaining positions by default

            // Create board with specified occupied positions
            var board = Board
                .Create(boardSize)
                .WithOccupied(occupiedPositions);

            // Add random occupied positions to board
            board = board.WithOccupied(
                board.Bounds
                    .EnumerateCoords()
                    .Where(coord => !board.IsOccupied(coord))
                    .Shuffle() // generate a random permutation of unoccupied positions
                    .Take(randomPositions)); // take the first N (if available)

            return board;
        }
    }
}
