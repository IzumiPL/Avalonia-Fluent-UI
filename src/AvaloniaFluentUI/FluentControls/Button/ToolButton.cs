using Avalonia;
using Avalonia.Controls;

namespace AvaloniaFluentUI.Controls;

public class ToolButton : Button
{
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
