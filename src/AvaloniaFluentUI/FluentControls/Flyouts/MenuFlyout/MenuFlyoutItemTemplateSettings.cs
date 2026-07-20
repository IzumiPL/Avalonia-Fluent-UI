using Avalonia;

namespace AvaloniaFluentUI.Controls;

/// <summary>
/// Defines objects used in the template of a <see cref="MenuFlyoutItem"/> and related classes
/// </summary>
public class MenuFlyoutItemTemplateSettings : AvaloniaObject
{
    /// <summary>
    /// Defines the <see cref="Icon"/> property
    /// </summary>
    public static readonly StyledProperty<IconElement> IconProperty =
        AvaloniaProperty.Register<MenuFlyoutItemTemplateSettings, IconElement>(nameof(Icon));

    /// <summary>
    /// Represents the FAIconElement for the MenuFlyoutItem
    /// </summary>
    public IconElement Icon
    {
        get => GetValue(IconProperty);
        internal set => SetValue(IconProperty, value);
    }
}
