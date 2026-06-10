using Avalonia;

namespace AvaloniaFluentUI.Controls;

/// <summary>
/// Provides calculated values that can be referenced as TemplatedParent sources when 
/// defining templates for a <see cref="TeachingTip"/>.
/// </summary>
public class TeachingTipTemplateSettings : AvaloniaObject
{
    /// <summary>
    /// Defines the <see cref="TopRightHighlightMargin"/> property
    /// </summary>
    public static readonly StyledProperty<Thickness> TopRightHighlighMarginProperty =
        AvaloniaProperty.Register<TeachingTipTemplateSettings, Thickness>(nameof(TopRightHighlightMargin));

    /// <summary>
    /// Defines the <see cref="TopLeftHighlightMargin"/> property
    /// </summary>
    public static readonly StyledProperty<Thickness> TopLeftHighlightMarginProperty =
        AvaloniaProperty.Register<TeachingTipTemplateSettings, Thickness>(nameof(TopLeftHighlightMargin));

    /// <summary>
    /// Defines the <see cref="IconElement"/> property
    /// </summary>
    public static readonly StyledProperty<IconElement> IconElementProperty =
        AvaloniaProperty.Register<TeachingTipTemplateSettings, IconElement>(nameof(IconElement));

    /// <summary>
    /// Gets the thickness value of the top right highlight margin.
    /// </summary>
    public Thickness TopRightHighlightMargin
    {
        get => GetValue(TopRightHighlighMarginProperty);
        internal set => SetValue(TopRightHighlighMarginProperty, value);
    }

    /// <summary>
    /// Gets the thickness value of the top left highlight margin.
    /// </summary>
    public Thickness TopLeftHighlightMargin
    {
        get => GetValue(TopLeftHighlightMarginProperty);
        internal set => SetValue(TopLeftHighlightMarginProperty, value);
    }

    /// <summary>
    /// Gets the icon element.
    /// </summary>
    public IconElement IconElement
    {
        get => GetValue(IconElementProperty);
        internal set => SetValue(IconElementProperty, value);
    }
}
