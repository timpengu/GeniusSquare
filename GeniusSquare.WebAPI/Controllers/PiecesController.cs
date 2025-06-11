using GeniusSquare.Core.Coords;
using GeniusSquare.WebAPI.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace GeniusSquare.WebAPI.Controllers;

[ApiController]
[Route("pieces")]
public class PiecesController : ControllerBase
{
    private readonly IDictionary<string, Model.Piece> _pieces;
    private readonly ILogger<PiecesController> _logger;

    public PiecesController(IEnumerable<Model.Piece> pieces, ILogger<PiecesController> logger)
    {
        _pieces = pieces.Normalise().ToDictionary(p => p.PieceId);
        _logger = logger;
    }

    [HttpGet()]
    public ActionResult<IEnumerable<Model.Piece>> Get()
    {
        return _pieces.Values.ToList();
    }

    [HttpGet("{pieceId}")]
    public ActionResult<Model.Piece> Get(string pieceId)
    {
        pieceId = pieceId.NormaliseId();

        if (!_pieces.TryGetValue(pieceId, out Model.Piece? piece))
        {
            return NotFound($"Unknown piece: '{pieceId}'");
        }

        return piece;
    }

    [HttpGet("{pieceId}/{orientationId}")]
    public ActionResult<Model.OrientedPiece> Get(string pieceId, string orientationId)
    {
        pieceId = pieceId.NormaliseId();
        orientationId = orientationId.NormaliseId();

        if (!_pieces.TryGetValue(pieceId, out Model.Piece? piece))
        {
            return NotFound($"Unknown piece: '{pieceId}'");
        }

        // TODO: Decouple WebAPI orientationId representation from Orientation enum?
        if (!Enum.TryParse(orientationId, ignoreCase:true, out Orientation orientation))
        {
            return NotFound($"Unknown orientation: '{orientationId}'");
        }

        List<Model.Coord> orientedPositions = piece.Positions
            .ToDomain()
            .Transform(orientation)
            .Normalise()
            .ToModel()
            .ToList();

        return new Model.OrientedPiece
        {
            PieceId = pieceId,
            OrientationId = orientationId,
            Positions = orientedPositions
        };
    }
}
