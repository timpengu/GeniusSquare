using GeniusSquare.Core.Coords;
using System.Diagnostics.CodeAnalysis;

namespace GeniusSquare.WebAPI.Helpers;

internal static class ParsingExtensions
{
    public static bool TryParseCoords(this string s, [NotNullWhen(true)] out List<Coord>? coords)
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
}
