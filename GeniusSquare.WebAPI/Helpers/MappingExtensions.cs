using GeniusSquare.Core.Coords;
using GeniusSquare.Core.Game;

namespace GeniusSquare.WebAPI.Helpers;

internal static class MappingExtensions
{
    public static IEnumerable<Coord> ToDomain(this IEnumerable<Model.Coord> source) => source.Select(coord => coord.ToDomain());
    public static IEnumerable<Model.Coord> ToModel(this IEnumerable<Coord> source) => source.Select(coord => coord.ToModel());

    public static Coord ToDomain(this Model.Coord coord) => new(coord.X, coord.Y);
    public static Model.Coord ToModel(this Coord coord) => new(coord.X, coord.Y);

    public static PieceOrientation ToDomain(this Model.PieceOrientation pieceOrientation) => pieceOrientation switch
    {
        Model.PieceOrientation.Original => PieceOrientation.Original,
        Model.PieceOrientation.Rotate => PieceOrientation.Rotate,
        Model.PieceOrientation.RotateAndReflect => PieceOrientation.RotateAndReflect,
        _ => throw new ArgumentException($"Invalid {nameof(Model.PieceOrientation)} value: {pieceOrientation}")
    };

    public static Model.Solution ToModel(this Solution solution, IBoardSize boardSize, string configId, int solutionNumber) => new Model.Solution
    {
        ConfigId = configId,
        SolutionNumber = solutionNumber,
        Placements = solution.Placements.ToModel().ToList(),
        LayoutPieceIds = boardSize.GetLayout(solution).ToModel(),
    };

    private static string[][] ToModel(this Piece?[,] layout)
    {
        int xSize = layout.GetLength(0);
        int ySize = layout.GetLength(1);

        return Enumerable.Range(0, ySize).Select(y =>
            Enumerable.Range(0, xSize).Select(x =>
                layout[x, y].ToModel()
            ).ToArray()
        ).ToArray();
    }

    private static string ToModel(this Piece? piece) => piece?.Name ?? string.Empty;

    public static IEnumerable<Model.Placement> ToModel(this IEnumerable<Placement> source) => source.Select(placement => placement.ToModel());

    public static Model.Placement ToModel(this Placement placement) => new Model.Placement
    {
        PieceId = placement.OrientedPiece.Piece.Name.NormaliseId(),
        OrientationId = placement.OrientedPiece.Orientation.ToString().NormaliseId(),
        Offset = placement.Offset.ToModel()
    };
}
