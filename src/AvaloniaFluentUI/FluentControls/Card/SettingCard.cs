using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AvaloniaFluentUI.Controls;

public class SettingCard : HeaderedContentControl
{
    public static readonly StyledProperty<string> DescriptionProperty =
        AvaloniaProperty.Register<SettingCard, string?>(nameof(Description));

    public static readonly StyledProperty<Geometry?> IconDataProperty =
        AvaloniaProperty.Register<SettingCard, Geometry?>(nameof(IconData));

    public Geometry? IconData
    {
        get => GetValue(IconDataProperty);
        set => SetValue(IconDataProperty, value);
    }
    
    public string? Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }
    
    public SettingCard()
    {
    }
}
