using Avalonia.Media;
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
    /// Gets or sets the background color of the title bar when the window is active
    /// </summary>
    public Color? BackgroundColor
    {
        get => _backgroundColor;
        set
        {
            if (_backgroundColor != value)
            {
                _backgroundColor = value;
                _parent.TitleBarColorsChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the foreground color of the title bar when the window is active
    /// </summary>
    public Color? ForegroundColor
    {
        get => _foregroundColor;
        set
        {
            if (_foregroundColor != value)
            {
                _foregroundColor = value;
                _parent.TitleBarColorsChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the background color of the title bar when the window is inactive
    /// </summary>
    public Color? InactiveBackgroundColor
    {
        get => _inactiveBackgroundColor;
        set
        {
            if (_inactiveBackgroundColor != value)
            {
                _inactiveBackgroundColor = value;
                _parent.TitleBarColorsChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the foreground color of the title bar when the window is inactive
    /// </summary>
    public Color? InactiveForegroundColor
    {
        get => _inactiveForegroundColor; 
        set
        {
            if (_inactiveForegroundColor != value)
            {
                _inactiveForegroundColor = value;
                _parent.TitleBarColorsChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the background color of the caption buttons when the window is active
    /// </summary>
    public Color? ButtonBackgroundColor
    {
        get => _buttonBackgroundColor; 
        set
        {
            if (_buttonBackgroundColor != value)
            {
                _buttonBackgroundColor = value;
                _parent.TitleBarColorsChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the foreground color of the caption buttons when the window is active
    /// </summary>
    public Color? ButtonForegroundColor
    {
        get => _buttonForegroundColor; 
        set
        {
            if (_buttonForegroundColor != value)
            {
                _buttonForegroundColor = value;
                _parent.TitleBarColorsChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the background color of the caption buttons when the window is active
    /// and the pointer is over the minimize or maximize button
    /// </summary>
    public Color? ButtonHoverBackgroundColor
    {
        get => _buttonHoverBackgroundColor;
        set
        {
            if (_buttonHoverBackgroundColor != value)
            {
                _buttonHoverBackgroundColor = value;
                _parent.TitleBarColorsChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the foreground color of the caption buttons when the window is active
    /// and the pointer is over the minimize or maximize button
    /// </summary>
    public Color? ButtonHoverForegroundColor
    {
        get => _buttonHoverForegroundColor; 
        set
        {
            if (_buttonHoverForegroundColor != value)
            {
                _buttonHoverForegroundColor = value;
                _parent.TitleBarColorsChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the background color of the caption buttons when the window is active
    /// and the pointer is pressed on the minimize or maximize button
    /// </summary>
    public Color? ButtonPressedBackgroundColor
    {
        get => _buttonPressedBackgroundColor;
        set
        {
            if (_buttonPressedBackgroundColor != value)
            {
                _buttonPressedBackgroundColor = value;
                _parent.TitleBarColorsChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the foreground color of the caption buttons when the window is active
    /// and the pointer is pressed on the minimize or maximize button
    /// </summary>
    public Color? ButtonPressedForegroundColor
    {
        get => _buttonPressedForegroundColor; 
        set
        {
            if (_buttonPressedForegroundColor != value)
            {
                _buttonPressedForegroundColor = value;
                _parent.TitleBarColorsChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the background color of the caption buttons when the window is inactive
    /// </summary>
    public Color? ButtonInactiveBackgroundColor
    {
        get => _buttonInactiveBackgroundColor; 
        set
        {
            if (_buttonInactiveBackgroundColor != value)
            {
                _buttonInactiveBackgroundColor = value;
                _parent.TitleBarColorsChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the foreground color of the caption buttons when the window is inactive
    /// </summary>
    public Color? ButtonInactiveForegroundColor
    {
        get => _buttonInactiveForegroundColor; 
        set
        {
            if (_buttonInactiveForegroundColor != value)
            {
                _buttonInactiveForegroundColor = value;
                _parent.TitleBarColorsChanged();
            }
        }
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
    
    // TODO: 未使用的配置
    private Color? _backgroundColor;
    private Color? _buttonBackgroundColor;
    private Color? _buttonForegroundColor;
    private Color? _buttonHoverBackgroundColor;
    private Color? _buttonHoverForegroundColor;
    private Color? _buttonInactiveBackgroundColor;
    private Color? _buttonInactiveForegroundColor;
    private Color? _buttonPressedBackgroundColor;
    private Color? _buttonPressedForegroundColor;
    // private bool _extendsContentIntoTitleBar;
    private Color? _foregroundColor;
    private Color? _inactiveBackgroundColor;
    private Color? _inactiveForegroundColor;
}
