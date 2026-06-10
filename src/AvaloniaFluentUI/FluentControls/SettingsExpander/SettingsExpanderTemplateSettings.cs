using Avalonia;

namespace AvaloniaFluentUI.Controls;

/// <summary>
/// Represents data for use in a SettingsExpander temlate
/// </summary>
public class SettingsExpanderTemplateSettings : AvaloniaObject
{
    internal SettingsExpanderTemplateSettings() { }

    /// <summary>
    /// Defines the <see cref="Icon"/> property
    /// </summary>
    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<SettingsExpanderTemplateSettings, object?>(nameof(Icon));

    /// <summary>
    /// Defines the <see cref="ActionIcon"/> property
    /// </summary>
    public static readonly StyledProperty<object?> ActionIconProperty =
        AvaloniaProperty.Register<SettingsExpanderTemplateSettings, object?>(nameof(ActionIcon));

    /// <summary>
    /// Defines the FAIconElement to be used for the SettingsExpander
    /// </summary>
    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// Defines the FAIconElement to be used for the SettingsExpander ActionIcon
    /// </summary>
    public object? ActionIcon
    {
        get => GetValue(ActionIconProperty);
        set => SetValue(ActionIconProperty, value);
    }
}
