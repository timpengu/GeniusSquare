using GeniusSquare.Core;
using GeniusSquare.Core.Coords;
using GeniusSquare.Core.Game;
using GeniusSquare.WebAPI.Caching;
using GeniusSquare.WebAPI.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;

namespace GeniusSquare.WebAPI.Controllers;

[ApiController]
[Route("solutions")]
public class SolutionsController : ControllerBase
{
    private readonly IDictionary<string, Model.Config> _configs;
    private readonly IDictionary<string, Model.Piece> _pieces;
    private readonly IAsyncCache<SolutionKey, IAsyncCachedEnumerable<Solution>> _cache;
    private readonly ILogger<ConfigController> _logger;

    public SolutionsController(
        IDictionary<string, Model.Config> configs,
        IDictionary<string, Model.Piece> pieces,
        IAsyncCache<SolutionKey, IAsyncCachedEnumerable<Solution>> cache,
        ILogger<ConfigController> logger)
    {
        _configs = configs;
        _pieces = pieces;
        _cache = cache;
        _logger = logger;
    }

    [HttpGet("{configId}/count")]
    public async Task<ActionResult<long>> GetCount(string configId, string? occ)
    {
        // Parse config ID
        configId = configId.NormaliseId();
        if (!_configs.TryGetValue(configId, out Model.Config? config))
        {
            return NotFound($"Unknown config: '{configId}'");
        }

        // Parse occupied positions
        List<Coord>? occupiedPositions = null;
        if (occ != null && !occ.TryParseCoords(out occupiedPositions))
        {
            return BadRequest($"Invalid occupied positions syntax: '{occ}'");
        }

        // Generate and count solutions
        long solutionCount = await
            GetSolutionsAsync(config, occupiedPositions)
            .LongCountAsync();

        return solutionCount;
    }

    [HttpGet("{configId}/{solutionNumber}")]
    public  async Task<ActionResult<Model.Solution>> Get(string configId, int solutionNumber, string? occ)
    {
        // Parse config ID
        configId = configId.NormaliseId();
        if (!_configs.TryGetValue(configId, out Model.Config? config))
        {
            return NotFound($"Unknown config: '{configId}'");
        }

        // Validate solution number
        if (solutionNumber < 1)
        {
            return BadRequest($"Invalid solution number: {solutionNumber}");
        }

        // Parse occupied positions
        List<Coord>? occupiedPositions = null;
        if (occ != null && !occ.TryParseCoords(out occupiedPositions))
        {
            return BadRequest($"Invalid occupied positions: '{occ}'");
        }

        // Generate, select and map requested solution
        Solution? solution = await
            GetSolutionsAsync(config, occupiedPositions)
            .Skip(solutionNumber - 1)
            .Select(solution => (Solution?)solution) // Solution is a struct
            .FirstOrDefaultAsync();

        // Get BoardSize from config
        // TODO: Refactor: Just pass a Coord boardSize to Layout. Drop IBoardSize.
        IBoardSize boardSize = BoardSize.FromCoord(config.BoardSize.ToDomain());

        return solution.HasValue
            ? solution.Value.ToModel(boardSize, configId, solutionNumber)
            : NotFound();
    }

    [HttpGet("{configId}")]
    public async Task<ActionResult<IEnumerable<Model.Solution>>> Get(string configId, string? occ, int ? skip = null, int? top = null)
    {
        // Parse config ID
        configId = configId.NormaliseId();
        if (!_configs.TryGetValue(configId, out Model.Config? config))
        {
            return NotFound($"Unknown config: '{configId}'");
        }

        // Parse occupied positions
        List<Coord>? occupiedPositions = null;
        if (occ != null && !occ.TryParseCoords(out occupiedPositions))
        {
            return BadRequest($"Invalid occupied positions syntax: '{occ}'");
        }

        // Get BoardSize from config
        // TODO: Refactor: Just pass a Coord boardSize to Layout. Drop IBoardSize.
        IBoardSize boardSize = BoardSize.FromCoord(config.BoardSize.ToDomain());

        // Generate solutions, page and map results
        int firstSolutionNumber = (skip ?? 0) + 1;
        List<Model.Solution> solutions = await
            GetSolutionsAsync(config, occupiedPositions)
            .Skip(skip)
            .Top(top)
            .Select((solution, index) => solution.ToModel(boardSize, configId, firstSolutionNumber + index))
            .ToListAsync();

        // TODO: Add prev,next links in Link header?
        // TODO: Add count in X-Total-Count header? (then need to enumerate all solutions)

        return solutions;
    }

    private async IAsyncEnumerable<Solution> GetSolutionsAsync(Model.Config config, List<Coord>? occupiedPositions)
    {
        // TODO: Implement limit on number of solutions that can be cached for an individual solution key
        IAsyncCachedEnumerable<Solution> GenerateCachedSolutions() =>
            GenerateSolutions(config, occupiedPositions)
            .ToAsyncCachedEnumerable();

        SolutionKey cacheKey = new(config.ConfigId, occupiedPositions ?? []);
        IAsyncCachedEnumerable<Solution> solutions = await _cache.GetOrAddAsync(cacheKey, GenerateCachedSolutions);

        await foreach (Solution solution in solutions)
        {
            yield return solution;
        }
    }

    private IEnumerable<Solution> GenerateSolutions(Model.Config config, List<Coord>? occupiedPositions)
    {
        // Generate board and pieces for this config
        Board board = Board.Create(config.BoardSize.ToDomain());
        IReadOnlyCollection<Piece> pieces = GeneratePieces(config).ToList();

        // Add occupied positions if specified
        if (occupiedPositions != null)
        {
            board = board.WithOccupiedPositions(occupiedPositions);
        }

        // Generate solutions
        return Solver.GetSolutions(board, pieces);
    }

    private IEnumerable<Piece> GeneratePieces(Model.Config config)
    {
        foreach (string pieceId in config.PieceIds)
        {
            if (!_pieces.TryGetValue(pieceId, out Model.Piece? configPiece))
            {
                // TODO: Validate PieceIds when creating configs
                throw new Exception($"Piece not found '{pieceId}' from config '{config.ConfigId}'");
            }

            Model.PieceAttributes attributes = configPiece.Attributes;

            ConsoleColor consoleColor = ConsoleColor.Black;
            if (attributes.ConsoleColor != null && !Enum.TryParse(attributes.ConsoleColor, ignoreCase:true, out consoleColor))
            {
                // TODO: Validate ConsoleColor when creating configs
                throw new Exception($"Invalid {nameof(ConsoleColor)} value: '{attributes.ConsoleColor}'");
            }

            Color htmlColor = Color.Black;
            if (attributes.HtmlColor != null)
            {
                // TODO: Validate HtmlColor when creating configs
                htmlColor = ColorTranslator.FromHtml(attributes.HtmlColor);
            }

            yield return PieceBuilder
                .Create(configPiece.PieceId)
                .WithAttributes(consoleColor, htmlColor)
                .WithPositions(configPiece.Positions.ToDomain())
                .WithOrientations(config.PieceOrientation.ToDomain())
                .BuildPiece();
        }
    }
}
