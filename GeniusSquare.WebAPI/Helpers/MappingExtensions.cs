using GeniusSquare.Core.Coords;
using GeniusSquare.Core.Game;

namespace GeniusSquare.WebAPI.Helpers;

// TODO: Use an AutoMapper framework to convert betwen WebAPI and coord models?
internal static class MappingExtensions
{
    public static IEnumerable<Coord> ToDomain(this IEnumerable<Model.Coord> source) => source.Select(coord => coord.ToDomain());
    public static IEnumerable<Model.Coord> ToModel(this IEnumerable<Coord> source) => source.Select(coord => coord.ToModel());

    public static Coord ToDomain(this Model.Coord coord) => new(coord.X, coord.Y);
    public static Model.Coord ToModel(this Coord coord) => new(coord.X, coord.Y);

    public static PieceTransformation ToDomain(this Model.PieceTransformation pieceTransformation) => pieceTransformation switch
    {
        Model.PieceTransformation.None => PieceTransformation.None,
        Model.PieceTransformation.Rotate => PieceTransformation.Rotate,
        Model.PieceTransformation.RotateAndReflect => PieceTransformation.RotateAndReflect,
        _ => throw new ArgumentException($"Invalid {nameof(Model.PieceTransformation)} value: {pieceTransformation}")
    };

    public static Model.Solution ToModel(this Solution solution, string configId, int solutionNumber) => new Model.Solution
    {
        ConfigId = configId,
        SolutionNumber = solutionNumber,
        Placements = solution.Placements.ToModel().ToList()
    };

    public static IEnumerable<Model.Placement> ToModel(this IEnumerable<Placement> source) => source.Select(placement => placement.ToModel());

    public static Model.Placement ToModel(this Placement placement) => new Model.Placement
    {
        PieceId = placement.OrientedPiece.Piece.Name.NormaliseId(),
        OrientationId = placement.OrientedPiece.Orientation.ToString().NormaliseId(),
        Offset = placement.Offset.ToModel()
    };
}
