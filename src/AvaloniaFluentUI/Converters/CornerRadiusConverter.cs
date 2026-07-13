using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace AvaloniaFluentUI.Converters;

public class CornerRadiusConverter : IValueConverter 
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is CornerRadius cr)
        {
            return new CornerRadius(0, 0, cr.BottomLeft + 1.5, cr.BottomRight + 1.5);
        }
        
        return new CornerRadius(0);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
