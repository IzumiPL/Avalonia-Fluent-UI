using System;
using Avalonia;
using Avalonia.Input;
using Avalonia.Layout;

namespace AvaloniaFluentUI.Controls;

public class SingleDirectionScrollContentPresenter : SmoothScrollContentPresenter 
{
    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<SingleDirectionScrollContentPresenter, Orientation>(nameof(Orientation));

    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }
    
    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        double step = -e.Delta.Y * SCROLL_STEP;

        if (_cts == null)
        {
            _targetOffset = Offset;
        }

        double max;
        double target;
        if (Orientation == Orientation.Horizontal)
        {
            max = Math.Max(0, Extent.Width - Viewport.Width);
            target = Math.Clamp(_targetOffset.X + step, 0, max);
            _targetOffset = _targetOffset.WithX(target);
        }
        else
        {
            max = Math.Max(0, Extent.Height - Viewport.Height);
            target = Math.Clamp(_targetOffset.Y + step, 0, max);
            _targetOffset = _targetOffset.WithY(target);
        }
            
        _ = ScrollToAsync(_targetOffset);
        e.Handled = true;
    }
}
