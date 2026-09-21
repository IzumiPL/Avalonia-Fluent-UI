using Avalonia;
using Avalonia.Media;

namespace AvaloniaFluentUI.Controls;

public class TabViewItemTemplateSettings : AvaloniaObject
{
    /// <summary>
    /// Defines the <see cref="TabGeometry"/> property
    /// </summary>
    public static readonly StyledProperty<Geometry> TabGeometryProperty =
        AvaloniaProperty.Register<TabViewItemTemplateSettings, Geometry>(nameof(TabGeometry));

    /// <summary>
    /// Gets the geometry of the current TabViewItem
    /// </summary>
    public Geometry TabGeometry
    {
        get => GetValue(TabGeometryProperty);
        internal set => SetValue(TabGeometryProperty, value);
    }
}
