using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Styling;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Media;

namespace AvaloniaFluentUI.Media.Animation;

public class FluentAnimation
{
    private static CancellationTokenSource _cancellationTokenSource;
    
    /// <summary>
    /// 对控件的指定属性执行渐变动画
    /// </summary>
    /// <param name="target">目标控件</param>
    /// <param name="property">要动画的属性</param>
    /// <param name="fromValue">起始值</param>
    /// <param name="toValue">结束值</param>
    /// <param name="duration">持续时间（毫秒）</param>
    public static async Task RunAnimateAsync(Animatable target, AvaloniaProperty property, object fromValue, object toValue, double duration = 250D)
    {
        var animation = new Avalonia.Animation.Animation
        {
            Duration = TimeSpan.FromMilliseconds(duration),
            Easing = new CubicEaseOut(),
            FillMode = FillMode.Forward,
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0D),
                    Setters = { new Setter(property, fromValue) }
                },
                new KeyFrame
                {
                    Cue = new Cue(1D),
                    Setters = { new Setter(property, toValue) }
                }
            }
        };

        await animation.RunAsync(target);
    }

    /// <summary>
    /// 淡入效果
    /// </summary>
    public static async void FadeInAsync(Visual target, double duration = 250D)
    {
        target.Opacity = 0;
        await RunAnimateAsync(target, Visual.OpacityProperty, 0D, 1D, duration);
    }

    /// <summary>
    /// 淡出效果
    /// </summary>
    public static async Task FadeOutAsync(Visual target, double duration = 250D)
    {
        await RunAnimateAsync(target, Visual.OpacityProperty, target.Opacity, 0D, duration);
        target.Opacity = 0;
    }

    public static async void CenterScaleAsync(Visual target, double offset, AvaloniaProperty? property = null, double duration = 200D)
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        
        target.Opacity = 0;
        property = property ?? ScaleTransform.ScaleYProperty;
        target.RenderTransformOrigin = new RelativePoint(1, 0.5, RelativeUnit.Relative);

        var animation = new Avalonia.Animation.Animation
        {
            Duration = TimeSpan.FromMilliseconds(duration),
            Easing = new SplineEasing(0.1, 0.9, 0.2, 1.0),
            FillMode = FillMode.Forward,
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0d),
                    Setters =
                    {
                        new Setter(Visual.OpacityProperty, 0d),
                        new Setter(property, offset)
                    }
                },
                new KeyFrame
                {
                    Cue = new Cue(1d),
                    Setters =
                    {
                        new Setter(Visual.OpacityProperty, 1d),
                        new Setter(property, 1d)
                    }
                }
            }
        };
        await animation.RunAsync(target, cancellationToken: _cancellationTokenSource.Token);
    }

    /// <summary>
    /// 从下方滑入
    /// </summary>
    public static async void SlideInAsync(
        Visual target, double offset, AvaloniaProperty? property = null, 
        double duration = 250D, Easing? easing = null 
        )
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        
        target.Opacity = 0;
        target.RenderTransform = new TranslateTransform(0, 0);
        
        property = property ?? TranslateTransform.YProperty;
        easing = easing ?? new CubicEaseOut();

        var animation = new Avalonia.Animation.Animation
        {
            Duration = TimeSpan.FromMilliseconds(duration),
            Easing = easing,
            FillMode = FillMode.Forward,
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0d),
                    Setters = { new Setter(Visual.OpacityProperty, 0.0), new Setter(property, offset), }
                },
                new KeyFrame
                {
                    Cue = new Cue(1d),
                    Setters = { new Setter(Visual.OpacityProperty, 1.0), new Setter(property, 0.0), }
                }
            }
        };
        await animation.RunAsync(target, cancellationToken: _cancellationTokenSource.Token);
    }
}
