using System;
using System.Threading;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Media;
using Avalonia.Styling;
using AvaloniaFluentUI.Animations;

namespace AvaloniaFluentUI.Controls;

public class FluentContextMenu : Avalonia.Controls.ContextMenu
{
    // private CancellationTokenSource? _cancellationTokenSource;

    public FluentContextMenu()
    {
        Opened += OnMenuOpened;
    }

    private void OnMenuOpened(object sender, EventArgs e)
    {
        _=FluentAnimation.ScaleAndSliderInAsync(this, -Bounds.Height);
        // _cancellationTokenSource?.Cancel();
        // _cancellationTokenSource = new CancellationTokenSource();
        //
        // RenderTransformOrigin =
        //     new RelativePoint(0.5, 0, RelativeUnit.Relative);
        //
        // var scale = new ScaleTransform(1.0, 1.0);
        // var translate = new TranslateTransform(0, 0);
        //
        // RenderTransform = new TransformGroup
        // {
        //     Children =
        //     {
        //         scale,
        //         translate
        //     }
        // };
        //
        // var animation = new Animation
        // {
        //     Duration = TimeSpan.FromMilliseconds(300),
        //     Easing = new SplineEasing(0.1, 0.9, 0.5, 1.0),
        //     FillMode = FillMode.Forward,
        //     Children =
        //     {
        //         new KeyFrame
        //         {
        //             Cue = new Cue(0.0),
        //             Setters =
        //             {
        //                 new Setter(OpacityProperty, 0.0),
        //                 new Setter(ScaleTransform.ScaleYProperty, 0.4),
        //                 new Setter(TranslateTransform.YProperty, -Bounds.Height)
        //             }
        //         },
        //         new KeyFrame
        //         {
        //             Cue = new Cue(1.0),
        //             Setters =
        //             {
        //                 new Setter(OpacityProperty, 1.0),
        //                 new Setter(ScaleTransform.ScaleYProperty, 1.0),
        //                 new Setter(TranslateTransform.YProperty, 0.0)
        //             }
        //         }
        //     }
        // };
        //
        // _=animation.RunAsync(this, cancellationToken: _cancellationTokenSource.Token);
    }
}
