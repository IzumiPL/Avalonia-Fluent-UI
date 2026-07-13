using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AvaloniaFluentUI.Controls;

public class SettingCard : HeaderedContentControl
{
    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<SettingCard, string?>(nameof(Description));

    public static readonly StyledProperty<object?> IconSourceProperty =
        AvaloniaProperty.Register<SettingCard, object?>(nameof(IconSource));

    public static readonly StyledProperty<bool> DescriptionIsVisibleProperty =
        AvaloniaProperty.Register<SettingCard, bool>(nameof(DescriptionIsVisible), true);

    public static readonly StyledProperty<double> IconSizeProperty =
        AvaloniaProperty.Register<SettingCard, double>(nameof(IconSize), 24);

    public double IconSize
    {
        get => GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

    public bool DescriptionIsVisible
    {
        get => GetValue(DescriptionIsVisibleProperty);
        set => SetValue(DescriptionIsVisibleProperty, value);
    }
    
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
}
