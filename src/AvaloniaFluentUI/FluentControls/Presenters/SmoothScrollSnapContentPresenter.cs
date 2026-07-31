using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Input;
using Avalonia.Layout;

namespace AvaloniaFluentUI.Controls;

public class SmoothScrollSnapContentPresenter : SmoothScrollContentPresenter
{
    // TODO: 使用反射获取ItemHeight在Aot模式下不可用,展示用固定值解决
    public double ItemHeight { get; set; } = 40;
    
    private const double MAX_DELTA = 600;

    protected override async Task Scroll(Orientation orientation)
    {
        // Cap delta before base scroll runs
        SetCurrentValue(OffsetProperty, Offset); // ensure Offset is up-to-date

        double itemHeight = ItemHeight;
        bool useSnap = itemHeight > 0;
        bool snapping = false;

        if (useSnap)
        {
            _isRunning = true;

            while (Math.Abs(_remainDelta) > 0.5)
            {
                // When remaining delta is small enough, redirect toward the nearest snap point
                if (!snapping && Math.Abs(_remainDelta) < itemHeight * 0.4)
                {
                    snapping = true;
                    double current = orientation == Orientation.Vertical ? Offset.Y : Offset.X;
                    double snapped = Math.Round(current / itemHeight) * itemHeight;
                    double max = orientation == Orientation.Vertical
                        ? Math.Max(0, Extent.Height - Viewport.Height)
                        : Math.Max(0, Extent.Width - Viewport.Width);
                    _remainDelta = Math.Clamp(snapped, 0, max) - current;
                }

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

            // Final correction to ensure exact alignment
            double cur = orientation == Orientation.Vertical ? Offset.Y : Offset.X;
            double snp = Math.Round(cur / itemHeight) * itemHeight;
            double mx = orientation == Orientation.Vertical
                ? Math.Max(0, Extent.Height - Viewport.Height)
                : Math.Max(0, Extent.Width - Viewport.Width);
            snp = Math.Clamp(snp, 0, mx);
            if (Math.Abs(snp - cur) > 0.1)
            {
                Vector v = orientation == Orientation.Vertical
                    ? Offset.WithY(snp)
                    : Offset.WithX(snp);
                SetCurrentValue(OffsetProperty, v);
            }

            _remainDelta = 0;
            _isRunning = false;
        }
        else
        {
            // No snap points detected, fall back to base behavior
            _remainDelta = Math.Clamp(_remainDelta, -MAX_DELTA, MAX_DELTA);
            await base.Scroll(orientation);
        }
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        _remainDelta += -e.Delta.Y * 60;
        _remainDelta = Math.Clamp(_remainDelta, -MAX_DELTA, MAX_DELTA);
        var direction = e.KeyModifiers.HasFlag(KeyModifiers.Alt) ? Orientation.Horizontal : Orientation.Vertical;
        if (!_isRunning) { _ = Scroll(direction); }

        e.Handled = true;
    }
}
