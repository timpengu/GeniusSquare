using GeniusSquare.WebAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace GeniusSquare.WebAPI.Controllers;

[ApiController]
[Route("pieces")]
public class PiecesController : ControllerBase
{
    private readonly IDictionary<string, Piece> _pieces;
    private readonly ILogger<PiecesController> _logger;

    public PiecesController(IDictionary<string, Piece> pieces, ILogger<PiecesController> logger)
    {
        _pieces = pieces;
        _logger = logger;
    }

    [HttpGet()]
    public ActionResult<IEnumerable<Piece>> Get()
    {
        return _pieces.Values.ToList();
    }

    [HttpGet("{id}")]
    public ActionResult<Piece> Get(string id)
    {
        if (!_pieces.TryGetValue(id, out Piece? piece))
        {
            return NotFound($"Unknown piece: '{id}'");
        }

        return piece;
    }
}
