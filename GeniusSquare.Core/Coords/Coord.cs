namespace GeniusSquare.Core.Coords;
public record struct Coord(int X, int Y) : IComparable<Coord>
{
    public static Coord Zero = new(0, 0); // default

    public static Coord MinValue = new(int.MinValue, int.MinValue);
    public static Coord MaxValue = new(int.MaxValue, int.MaxValue);

    public static Coord operator +(Coord a, Coord b) => new(a.X + b.X, a.Y + b.Y);
    public static Coord operator -(Coord a, Coord b) => new(a.X - b.X, a.Y - b.Y);
    public static Coord operator -(Coord a) => Zero - a;

    public static Coord Min(Coord a, Coord b) => new(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
    public static Coord Max(Coord a, Coord b) => new(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));

    public int CompareTo(Coord other)
    {
        int compareX = X.CompareTo(other.X);
        if (compareX != 0) return compareX;

        int compareY = Y.CompareTo(other.Y);
        if (compareY != 0) return compareY;

        return 0;
    }

    public override string ToString() => $"({X},{Y})";
}
