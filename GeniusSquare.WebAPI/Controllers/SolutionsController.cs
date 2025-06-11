using GeniusSquare.Core;
using GeniusSquare.Core.Coords;
using GeniusSquare.Core.Game;
using GeniusSquare.WebAPI.Helpers;
using Microsoft.AspNetCore.Mvc;
using MoreLinq;
using System.Drawing;

namespace GeniusSquare.WebAPI.Controllers;

[ApiController]
[Route("solutions")]
public class SolutionsController : ControllerBase
{
    private readonly IDictionary<string, Model.Config> _configs;
    private readonly IDictionary<string, Model.Piece> _pieces;
    private readonly ILogger<ConfigController> _logger;

    public SolutionsController(
        IEnumerable<Model.Config> configs,
        IEnumerable<Model.Piece> pieces,
        ILogger<ConfigController> logger)
    {
        // TODO: Normalise models before injecting to controllers
        _configs = configs.Normalise().ToDictionary(c => c.ConfigId);
        _pieces = pieces.Normalise().ToDictionary(p => p.PieceId);
        _logger = logger;
    }

    [HttpGet("{configId}/count")]
    public ActionResult<long> GetCount(string configId, string? occ)
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
        long solutionCount = GenerateSolutions(config, occupiedPositions).LongCount();

        return solutionCount;
    }

    [HttpGet("{configId}/{solutionNumber}")]
    public ActionResult<Model.Solution> Get(string configId, int solutionNumber, string? occ)
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
        Model.Solution? solution = GenerateSolutions(config, occupiedPositions)
            .Skip(solutionNumber - 1)
            .Cast<Solution?>() // Solution is a struct
            .FirstOrDefault()
            ?.ToModel(configId, solutionNumber);

        return solution != null ? solution : NotFound();
    }

    [HttpGet("{configId}")]
    public ActionResult<IEnumerable<Model.Solution>> Get(string configId, string? occ, int ? skip = null, int? top = null)
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

        // Generate solutions, page and map results
        int firstSolutionNumber = (skip ?? 0) + 1;
        List<Model.Solution> solutions = GenerateSolutions(config, occupiedPositions)
            .Skip(skip)
            .Top(top)
            .Select((solution, index) => solution.ToModel(configId, firstSolutionNumber + index))
            .ToList();

        // TODO: Add prev,next links in Link header?
        // TODO: Add count in X-Total-Count header? (then need to enumerate all solutions)

        return solutions;
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
        // TODO: Cache solutions per (config,occupiedPositions)
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

            ConsoleColor consoleColor = ConsoleColor.Black;
            if (configPiece.ConsoleColor != null && !Enum.TryParse(configPiece.ConsoleColor, ignoreCase:true, out consoleColor))
            {
                // TODO: Validate ConsoleColor when creating configs
                throw new Exception($"Invalid {nameof(ConsoleColor)} value: '{configPiece.ConsoleColor}'");
            }

            Color htmlColor = Color.Black;
            if (configPiece.HtmlColor != null)
            {
                // TODO: Validate HtmlColor when creating configs
                htmlColor = ColorTranslator.FromHtml(configPiece.HtmlColor);
            }

            yield return PieceBuilder
                .Create(configPiece.PieceId, consoleColor, htmlColor)
                .WithPositions(configPiece.Positions.ToDomain())
                .WithOrientations(config.PieceOrientation.ToDomain())
                .BuildPiece();
        }
    }
}
