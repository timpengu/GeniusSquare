using GeniusSquare.Core.Coords;
using GeniusSquare.WebAPI.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

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

        if (!TryGetOrientation(orientationId, out Orientation? orientation))
        {
            return NotFound($"Unknown orientation: '{orientationId}'");
        }

        List<Model.Coord> orientedPositions = piece.Positions
            .ToDomain()
            .Transform(orientation.Value)
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

    // TODO: Encapsulate orientation ID mapping in GeniusSquare.Core (see PieceBuilder)
    private static bool TryGetOrientation(string orientationId, [NotNullWhen(true)] out Orientation? orientation)
    {
        orientation = orientationId switch
        {
            "a" or "ar" => Orientation.Ar,
            "b" or "br" => Orientation.Br,
            "c" or "cr" => Orientation.Cr,
            "d" or "dr" => Orientation.Dr,
            "al" => Orientation.Al,
            "bl" => Orientation.Bl,
            "cl" => Orientation.Cl,
            "dl" => Orientation.Dl,
            _ => null
        };

        return orientation != null;
    }
}
