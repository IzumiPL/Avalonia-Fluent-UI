using Avalonia;

namespace AvaloniaFluentUI.Controls;

/// <summary>
/// Stores settings for use in the template of a CommandBarButton
/// </summary>
public class CommandBarButtonTemplateSettings : AvaloniaObject
{
    /// <summary>
    /// Defines the <see cref="Icon"/> property
    /// </summary>
    public static readonly StyledProperty<IconElement> IconProperty =
        MenuFlyoutItemTemplateSettings.IconProperty.AddOwner<CommandBarButtonTemplateSettings>();

    /// <summary>
    /// Gets the Icon for the CommandBarButton
    /// </summary>
    public IconElement Icon
    {
        get => GetValue(IconProperty);
        internal set => SetValue(IconProperty, value);
    }
}
