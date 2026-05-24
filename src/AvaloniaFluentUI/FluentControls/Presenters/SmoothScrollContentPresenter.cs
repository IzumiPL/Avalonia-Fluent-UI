using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Input;
using Avalonia.Layout;

namespace AvaloniaFluentUI.Controls;


public class SmoothScrollContentPresenter : Avalonia.Controls.Presenters.ScrollContentPresenter
{
    private double _remainDelta;
    private bool _isRunning;

    private async Task Scroll(Orientation orientation)
    {
        _isRunning = true;
    
        while (Math.Abs(_remainDelta) > 0.5)
        {
            double delta = _remainDelta * 0.25;
            _remainDelta -= delta;
            Vector vector;
            if (orientation == Orientation.Horizontal)
            {
                double target = Offset.X + delta;
                double max = Math.Max(0, Extent.Width - Viewport.Width);
                vector = Offset.WithX(Math.Clamp(target, 0, max));
            }
            else
            {
                double target = Offset.Y + delta;
                double max = Math.Max(0, Extent.Height - Viewport.Height);
                vector = Offset.WithY(Math.Clamp(target, 0, max));
            }
    
            SetCurrentValue(OffsetProperty, vector);
    
            await Task.Delay(8);
        }
    
        _remainDelta = 0;
        _isRunning = false;
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        _remainDelta += -e.Delta.Y * 60;
        var direction = e.KeyModifiers.HasFlag(KeyModifiers.Alt) ? Orientation.Horizontal : Orientation.Vertical;
        if (!_isRunning) { _=Scroll(direction);}

        e.Handled = true;
        // base.OnPointerWheelChanged(e);
    }
}
