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
        _pieces = pieces.ToDictionary(p => p.Id.NormaliseId());
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
}
