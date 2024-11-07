namespace GeniusSquare.Coords;
public record struct Coord(int X, int Y) : IComparable<Coord>
{
    public static Coord Zero = new Coord(0, 0); // default

    public static Coord operator +(Coord a, Coord b) => new(a.X + b.X, a.Y + b.Y);
    public static Coord operator -(Coord a, Coord b) => new(a.X - b.X, a.Y - b.Y);
    public static Coord operator -(Coord a) => Zero - a;

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
