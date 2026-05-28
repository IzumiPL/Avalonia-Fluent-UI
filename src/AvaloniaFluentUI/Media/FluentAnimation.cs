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
    public static async void RunAnimateAsync(Animatable target, AvaloniaProperty property, object fromValue, object toValue, double duration = 250D)
    {
        var animation = new Avalonia.Animation.Animation
        {
            Duration = TimeSpan.FromMilliseconds(duration),
            Easing = new CubicEaseOut(),
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
    public static void FadeInAsync(Visual target, double duration = 250D)
    {
        target.Opacity = 0;
        RunAnimateAsync(target, Visual.OpacityProperty, 0D, 1D, duration);
    }

    /// <summary>
    /// 淡出效果
    /// </summary>
    public static void FadeOutAsync(Visual target, double duration = 250D)
    {
        RunAnimateAsync(target, Visual.OpacityProperty, target.Opacity, 0D, duration);
        target.Opacity = 0;
    }

    public static async void ScaleAndSliderInAsync(Visual target, double sliderOffset, double scaleOffset = 0.4D, double duration = 300D)
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();

        target.RenderTransformOrigin = new RelativePoint(0.5, 0, RelativeUnit.Relative);
        target.RenderTransform = new TransformGroup
        {
            Children =
            {
                new ScaleTransform(1.0, 1.0), 
                new TranslateTransform(0, 0)
            }
        };

        var animation = new Avalonia.Animation.Animation
        {
            Duration = TimeSpan.FromMilliseconds(300),
            Easing = new SplineEasing(0.1, 0.9, 0.5, 1.0),
            FillMode = FillMode.Forward,
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0.0),
                    Setters =
                    {
                        new Setter(Visual.OpacityProperty, 0.5),
                        new Setter(ScaleTransform.ScaleYProperty, scaleOffset),
                        new Setter(TranslateTransform.YProperty, sliderOffset)
                    }
                },
                new KeyFrame
                {
                    Cue = new Cue(1.0),
                    Setters =
                    {
                         new Setter(Visual.OpacityProperty, 1.0),
                        new Setter(ScaleTransform.ScaleYProperty, 1.0),
                        new Setter(TranslateTransform.YProperty, 0.0) 
                    }
                }
            }
        };
        await animation.RunAsync(target, cancellationToken: _cancellationTokenSource.Token);
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
            Duration = TimeSpan.FromMilliseconds(200),
            // Easing = new QuarticEaseOut(),
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
                        new Setter(ScaleTransform.ScaleYProperty, offset)
                    }
                },
                new KeyFrame
                {
                    Cue = new Cue(1d),
                    Setters =
                    {
                        new Setter(Visual.OpacityProperty, 1d),
                        new Setter(ScaleTransform.ScaleYProperty, 1d)
                    }
                }
            }
        };
        await animation.RunAsync(target, cancellationToken: _cancellationTokenSource.Token);
    }

    /// <summary>
    /// 从下方滑入
    /// </summary>
    public static async void SlideInAsync(Visual target, double offset, AvaloniaProperty? property = null, double duration = 250D)
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        
        target.Opacity = 0;
        property = property ?? TranslateTransform.YProperty;

        var animation = new Avalonia.Animation.Animation
        {
            Duration = TimeSpan.FromMilliseconds(duration),
            Easing = new CubicEaseOut(),
            FillMode = FillMode.Forward,
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0d),
                    Setters = { new Setter(Visual.OpacityProperty, 0d), new Setter(property, offset), }
                },
                new KeyFrame
                {
                    Cue = new Cue(1d),
                    Setters = { new Setter(Visual.OpacityProperty, 1d), new Setter(property, 0d), }
                }
            }
        };
        await animation.RunAsync(target, cancellationToken: _cancellationTokenSource.Token);
    }
}
