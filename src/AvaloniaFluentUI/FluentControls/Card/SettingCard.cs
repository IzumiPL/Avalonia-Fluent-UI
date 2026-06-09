using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AvaloniaFluentUI.Controls;

public class SettingCard : HeaderedContentControl
{
    public static readonly StyledProperty<string> DescriptionProperty =
        AvaloniaProperty.Register<SettingCard, string?>(nameof(Description));

    public static readonly StyledProperty<object?> IconSourceProperty =
        AvaloniaProperty.Register<SettingCard, object?>(nameof(IconSource));

    public object? IconSource 
    {
        get => GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
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
