using System;
using System.Globalization;
using System.IO;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;

namespace AvaloniaFluentUI.Converters;

public class WindowIconToImageConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is WindowIcon windowIcon)
        {
            using var stream = new MemoryStream();
            windowIcon.Save(stream);
            stream.Position = 0;
            
            return new Bitmap(stream);
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
