using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;

namespace AvaloniaFluentUI.Converters;

/// <summary>
/// Converts a ProgressRing value to a percentage string.
/// Parameter should be the Maximum value (e.g. "100").
/// </summary>
public class ProgressPercentConverter : IMultiValueConverter
{
    public static readonly ProgressPercentConverter Instance = new();

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 2 || values[0] is not double value || values[1] is not double maximum)
            return string.Empty;

        if (maximum <= 0)
            return "0%";

        var percent = (double)(value / maximum * 100);
        return $"{percent:F1}%";
    }
}
