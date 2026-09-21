using System;
using System.Threading.Tasks;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Layout;

namespace AvaloniaFluentUI.Controls;

/// <summary>
/// Popup-style info bar with icon + colored-dot severity indicator.
/// Visual template is defined in <c>PopupInfoBar.axaml</c>.
/// </summary>
public class PopupInfoBar : InfoBarBase
{
    protected override ColumnDefinitions GetColumnDefinitions()
    {
        return Orientation == Orientation.Horizontal ? new ColumnDefinitions("Auto, Auto, *, Auto") : new ColumnDefinitions("Auto, *, Auto");
    }

    protected override RowDefinitions GetRowDefinitions()
    {
        return Orientation == Orientation.Horizontal ? new RowDefinitions() : new RowDefinitions("32, *");
    }

    public override void Run(double fromX, double fromY, double toX, double toY)
    {
        OffsetX = fromX;
        OffsetY = fromY;

        var duration = TimeSpan.FromMilliseconds(AnimationDuration);
        Transitions = new Transitions
        {
            new DoubleTransition
            {
                Property = OffsetXProperty,
                Duration = duration,
                Easing = new CubicEaseOut(),
            },
            new DoubleTransition
            {
                Property = OffsetYProperty,
                Duration = duration,
                Easing = new CubicEaseOut(),
            },
            new DoubleTransition
            {
                Property = OpacityProperty,
                Duration = duration,
            },
        };

        OffsetX = toX;
        OffsetY = toY;
        Opacity = 1;
    }

    public override Task CloseAsync(double x, double y)
    {
        Opacity = 0;
        return base.CloseAsync(x, y);
    }
}
