using GeniusSquare.Core;
using GeniusSquare.Core.Coords;
using GeniusSquare.Core.Game;
using GeniusSquare.WebAPI.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

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

    [HttpGet("{configId}")]
    public ActionResult<IEnumerable<Model.Solution>> Get(string configId, string? occPos, int? occRnd, int? skip, int? top)
    {
        configId = configId.NormaliseId();

        if (!_configs.TryGetValue(configId, out Model.Config? config))
        {
            return NotFound($"Unknown config: '{configId}'");
        }

        // Generate pieces from config
        IReadOnlyCollection<Piece> pieces = GeneratePieces(config).ToList();

        // Create board with configured size
        Board board = Board.Create(
            config.BoardSize.ToDomain()
        );

        // Add occupied positions if specified
        if (occPos != null)
        {
            if (!TryParseCoords(occPos, out List<Coord>? occupiedPositions))
            {
                return BadRequest($"Invalid parameter {nameof(occPos)}: '{occPos}'");
            }

            board = board.WithOccupiedPositions(occupiedPositions);
        }

        // Add random occupied positions 
        board = board.WithOccupiedRandomPositions(
            occRnd ?? // add specified number of random occupied positions, or default to..
            board.SurplusPositions(pieces) // critically constrain the board (leave zero unoccupied positions when all pieces are placed)
        );

        // Solve and page solutions
        IEnumerable<Solution> solutions = Solver
            .GetSolutions(board, pieces)
            .Skip(skip)
            .Top(top);

        // TODO: Map solutions to WebAPI model
        return new List<Model.Solution>();
    }

    private IEnumerable<Piece> GeneratePieces(Model.Config config)
    {
        foreach (string pieceId in config.PieceIds)
        {
            if (!_pieces.TryGetValue(pieceId, out Model.Piece? configPiece))
            {
                throw new Exception($"Piece not found '{pieceId}' from config '{config.ConfigId}'");
            }

            ConsoleColor consoleColor = ParseConsoleColor(configPiece.ConsoleColor) ?? ConsoleColor.White;

            // TODO: Parse configPiece.HtmlColor

            yield return PieceBuilder
                .Create(configPiece.PieceId, consoleColor)
                .WithPositions(configPiece.Positions.ToDomain())
                .WithOrientations(config.PieceTransformation.ToDomain())
                .BuildPiece();
        }
    }

    private static bool TryParseCoords(string s, [NotNullWhen(true)] out List<Coord>? coords)
    {
        string[] strCoords = s.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        coords = new List<Coord>(strCoords.Length);
        foreach (var strCoord in strCoords)
        {
            if (!Coord.TryParse(strCoord, out Coord coord))
            {
                coords = default;
                return false;
            }
            coords.Add(coord);
        }
        return true;
    }

    private static ConsoleColor? ParseConsoleColor(string? consoleColor) =>
        consoleColor == null ? null : Enum.Parse<ConsoleColor>(consoleColor);
}
