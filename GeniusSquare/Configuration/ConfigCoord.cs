using GeniusSquare.Coords;

namespace GeniusSquare.Configuration;

public sealed class ConfigCoord : List<int>
{
    public int X => GetElement(0);
    public int Y => GetElement(1);

    private int GetElement(int index)
    {
        if (Count == 0)
            throw new FormatException("Empty coords array");

        if (Count != 2)
            throw new FormatException($"Invalid coords array (size {Count})");

        return this[index];
    }
}