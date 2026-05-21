using Avalonia;
using Avalonia.Media;

namespace AvaloniaFluentUI.Controls;

public class PushButton : Avalonia.Controls.Button
{
    public static readonly StyledProperty<Geometry> IconDataProperty =
        AvaloniaProperty.Register<PushButton, Geometry?>(nameof(IconData));

    public Geometry? IconData
    {
        get => GetValue(IconDataProperty);
        set => SetValue(IconDataProperty, value);
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
