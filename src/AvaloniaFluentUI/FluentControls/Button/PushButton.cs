using Avalonia;
using Avalonia.Controls;

namespace AvaloniaFluentUI.Controls;

public class PushButton : Button
{
    public static readonly StyledProperty<object?> IconSourceProperty =
        AvaloniaProperty.Register<PushButton, object?>(nameof(IconSource));

    public object? IconSource
    {
        get => GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }

    public static readonly StyledProperty<double> IconWidthProperty =
        AvaloniaProperty.Register<PushButton, double>(nameof(IconWidth));

    public double IconWidth
    {
        get => GetValue(IconWidthProperty);
        set => SetValue(IconWidthProperty, value);
    }

    public static readonly StyledProperty<double> IconHeightProperty =
        AvaloniaProperty.Register<PushButton, double>(nameof(IconHeight));

    public double IconHeight
    {
        get => GetValue(IconHeightProperty);
        set => SetValue(IconHeightProperty, value);
    }
}
