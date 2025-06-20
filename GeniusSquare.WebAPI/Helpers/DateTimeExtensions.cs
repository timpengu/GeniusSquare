namespace GeniusSquare.WebAPI.Helpers;

public static class DateTimeExtensions
{
    public static DateTime RoundUp(this DateTime dt, TimeSpan d) =>
        new DateTime(
            Math.Min((dt.Ticks + d.Ticks - 1) / d.Ticks * d.Ticks, DateTime.MaxValue.Ticks),
            dt.Kind);
}
