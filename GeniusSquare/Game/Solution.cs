namespace GeniusSquare.Game;

public record struct Solution(IReadOnlyCollection<Placement> Placements)
{
    public override string ToString() => string.Join(", ", Placements);
}
