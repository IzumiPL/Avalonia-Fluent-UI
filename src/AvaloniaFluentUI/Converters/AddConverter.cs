using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace AvaloniaFluentUI.Converters;

public class AddConverter : IValueConverter 
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double num)
        {
            if (parameter != null && double.TryParse(parameter.ToString(), out var p))
            {
                num += p;
            }

            return num;
        }

        return value;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
