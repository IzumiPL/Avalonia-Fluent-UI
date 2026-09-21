using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Input;
using Avalonia.Styling;

namespace AvaloniaFluentUI.Controls;

public class SmoothScrollContentPresenter : Avalonia.Controls.Presenters.ScrollContentPresenter
{
    protected Vector _targetOffset;
    protected CancellationTokenSource? _cts;

    protected const double SCROLL_STEP = 60;

    public async Task ScrollToAsync(Vector target)
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = new CancellationTokenSource();

        var anim = new Animation
        {
            Duration = TimeSpan.FromMilliseconds(150),
            Easing = new QuadraticEaseOut(),
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(1.0),
                    Setters = { new Setter(OffsetProperty, target) },
                },
            },
        };

        try
        {
            await anim.RunAsync(this, _cts.Token);
        }
        finally
        {
            if (!_cts.IsCancellationRequested)
            {
                _cts = null;
            }
        }
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
        var horizontal = e.KeyModifiers.HasFlag(KeyModifiers.Alt);

        if (horizontal)
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
