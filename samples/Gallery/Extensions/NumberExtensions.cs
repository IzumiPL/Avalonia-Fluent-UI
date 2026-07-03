namespace Gallery.Extensions;

using System.Globalization;

public static class NumericExtensions
{
    public static int ToIntOrZero(this string? value)
    {
        return int.TryParse(value, NumberStyles.Integer,
            CultureInfo.InvariantCulture, out var result)
            ? result
            : 0;
    }

    public static int ToIntOrDefault(this string? value, int defaultValue)
    {
        return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result)
            ? result
            : defaultValue;
    }

    public static double ToDoubleOrZero(this string? value)
    {
        return double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands,
            CultureInfo.InvariantCulture, out var result)
            ? result
            : 0d;
    }

    public static double ToDoubleOrNan(this string? value)
    {
        return double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InstalledUICulture, out var result) ? result : double.NaN;
    }
}
