using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace AvaloniaFluentUI.Converters;

public class CornerRadiusAdjustConverter : IValueConverter 
{
    public bool KeepTop { get; set; } = true;
    public double TopOffset { get; set; } = 0;
    public double BottomOffset { get; set; } = 2;
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not CornerRadius radius)
        {
            return new CornerRadius();
        }
        
        return new CornerRadius(
            KeepTop ? radius.TopLeft + TopOffset : TopOffset,
            KeepTop ? radius.TopRight + TopOffset : TopOffset,
            radius.BottomLeft + BottomOffset,
            radius.BottomRight + BottomOffset);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
