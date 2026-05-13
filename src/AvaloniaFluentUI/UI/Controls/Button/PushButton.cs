using Avalonia;
using Avalonia.Media;

namespace AvaloniaFluentUI.UI.Controls;

public class PushButton : Avalonia.Controls.Button
{
    public static readonly StyledProperty<Geometry> IconDataProperty =
        AvaloniaProperty.Register<PushButton, Geometry?>(nameof(IconData));

    public Geometry? IconData
    {
        get => GetValue(IconDataProperty);
        set => SetValue(IconDataProperty, value);
    }
}
