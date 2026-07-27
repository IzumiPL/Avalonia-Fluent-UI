using AvaloniaFluentUI.Core;

namespace AvaloniaFluentUI.Windowing;

/// <summary>
/// Represents the title bar of an <see cref="FluentWindow"/> allowing customization such as
/// colors, hit testing, and allowing app content in the title bar area
/// </summary>

public class FluentWindowTitleBar
{
    internal FluentWindowTitleBar(FluentWindow parent)
    {
        _parent = parent;
    }

    /// <summary>
    /// Gets or sets the height of the default title bar
    /// </summary>
    /// <remarks>
    /// default drag rect and caption buttons only. If custom drag rects are set, only the caption
    /// buttons are affected by this
    /// </remarks>
    public double Height
    {
        get => _height;
        set
        {
            if (!MathHelpers.IsClose(_height, value))
            {
                _height = value;
                _parent.OnTitleBarHeightChanged(value);
            }
        }
    }

    private FluentWindow _parent;
    private double _height = 45;
}
