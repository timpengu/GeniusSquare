using System.Drawing;

namespace GeniusSquare.Core.Game;

public sealed record PieceAttributes(
    ConsoleColor ConsoleColor = default,
    Color HtmlColor = default)
{
}
