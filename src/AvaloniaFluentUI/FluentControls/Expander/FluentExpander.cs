using System;
using System.Threading;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Styling;

namespace AvaloniaFluentUI.Controls;

/// <summary>
/// An Expander control with WinUI-like expand/collapse animations,
/// supporting all four expand directions.
/// </summary>
[TemplatePart(Name =  PART_EXPANDER_CONTENT, Type = typeof(Border))]
public class FluentExpander : Expander
{
    private Border? _expanderContent;
    private CancellationTokenSource? _cts;
    private Size _contentSize;
    private readonly TranslateTransform _translateTransform = new();

    private const string PART_EXPANDER_CONTENT = "PART_ExpanderContent";

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        
        _expanderContent?.SizeChanged -= OnContentSizeChanged;

        _expanderContent = e.NameScope.Find<Border>(PART_EXPANDER_CONTENT);
        _expanderContent?.SizeChanged += OnContentSizeChanged;

        UpdateState(false);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsExpandedProperty)
        {
            UpdateState(true);
        }
    }

    private void OnContentSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        _contentSize = e.NewSize;
    }

    private void UpdateState(bool useTransitions)
    {
        if (_expanderContent == null)
            return;

        var expanded = IsExpanded;

        if (!useTransitions)
        {
            _expanderContent.IsVisible = expanded;
            return;
        }

        var direction = ExpandDirection;

        if (expanded)
        {
            switch (direction)
            {
                case ExpandDirection.Down:
                case ExpandDirection.Up:
                    RunExpandDownUpAnimation(direction == ExpandDirection.Down);
                    break;

                case ExpandDirection.Left:
                case ExpandDirection.Right:
                    RunExpandLeftRightAnimation(direction == ExpandDirection.Right);
                    break;
            }
        }
        else
        {
            switch (direction)
            {
                case ExpandDirection.Down:
                case ExpandDirection.Up:
                    RunCollapseDownUpAnimation(direction == ExpandDirection.Down);
                    break;

                case ExpandDirection.Left:
                case ExpandDirection.Right:
                    RunCollapseLeftRightAnimation(direction == ExpandDirection.Right);
                    break;
            }
        }
    }

    private async void RunExpandDownUpAnimation(bool down)
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        _expanderContent!.RenderTransform = _translateTransform;
        _expanderContent.IsVisible = true;

        if (Parent is ExpanderSettingCard se && se.Presenter != null)
        {
            // ExpanderSettingCard does not use virtualization, so it's safe to use Infinity to measure
            se.Presenter.Measure(Size.Infinity);
            _contentSize = se.Presenter.DesiredSize;
        }
        else
        {
            _expanderContent.Measure(Size.Infinity);
            _contentSize = _expanderContent.DesiredSize;
        }

        var startY = down ? -_contentSize.Height : _contentSize.Height;
        var animation = new Animation
        {
            Duration = TimeSpan.FromMilliseconds(333),
            FillMode = FillMode.Forward,
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0),
                    Setters =
                    {
                        new Setter(TranslateTransform.YProperty, startY)
                    }
                },
                new KeyFrame
                {
                    Cue = new Cue(1),
                    Setters =
                    {
                        new Setter(TranslateTransform.YProperty, 0d)
                    },
                    KeySpline = new KeySpline(0, 0, 0, 1)
                }
            }
        };

        await animation.RunAsync(_expanderContent, _cts.Token);

        _translateTransform.Y = 0;
    }

    private async void RunCollapseDownUpAnimation(bool down)
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        _expanderContent!.RenderTransform = _translateTransform;

        var endY = down ? -_contentSize.Height : _contentSize.Height;
        var animation = new Animation
        {
            Duration = TimeSpan.FromMilliseconds(167),
            FillMode = FillMode.Forward,
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0),
                    Setters =
                    {
                        new Setter(TranslateTransform.YProperty, 0d)
                    }
                },
                new KeyFrame
                {
                    Cue = new Cue(1),
                    Setters =
                    {
                        new Setter(TranslateTransform.YProperty, endY),
                        new Setter(IsVisibleProperty, false)
                    },
                    KeySpline = new KeySpline(1, 1, 0, 1)
                }
            }
        };

        await animation.RunAsync(_expanderContent, _cts.Token);

        _expanderContent.IsVisible = false;
        _translateTransform.Y = 0;
    }

    private async void RunExpandLeftRightAnimation(bool right)
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        _expanderContent!.RenderTransform = _translateTransform;
        _expanderContent.IsVisible = true;
        _expanderContent.Measure(Size.Infinity);
        _contentSize = _expanderContent.DesiredSize;

        var startX = right ? -_contentSize.Width : _contentSize.Width;
        var animation = new Animation
        {
            Duration = TimeSpan.FromMilliseconds(333),
            FillMode = FillMode.Forward,
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0),
                    Setters =
                    {
                        new Setter(TranslateTransform.XProperty, startX)
                    }
                },
                new KeyFrame
                {
                    Cue = new Cue(1),
                    Setters =
                    {
                        new Setter(TranslateTransform.XProperty, 0d)
                    },
                    KeySpline = new KeySpline(0, 0, 0, 1)
                }
            }
        };

        await animation.RunAsync(_expanderContent, _cts.Token);

        _translateTransform.X = 0;
    }

    private async void RunCollapseLeftRightAnimation(bool right)
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        _expanderContent!.RenderTransform = _translateTransform;

        var endX = right ? -_contentSize.Width : _contentSize.Width;
        var animation = new Animation
        {
            Duration = TimeSpan.FromMilliseconds(167),
            FillMode = FillMode.Forward,
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0),
                    Setters =
                    {
                        new Setter(TranslateTransform.XProperty, 0d)
                    }
                },
                new KeyFrame
                {
                    Cue = new Cue(1),
                    Setters =
                    {
                        new Setter(TranslateTransform.XProperty, endX),
                        new Setter(IsVisibleProperty, false)
                    },
                    KeySpline = new KeySpline(1, 1, 0, 1)
                }
            }
        };

        await animation.RunAsync(_expanderContent, _cts.Token);

        _expanderContent.IsVisible = false;
        _translateTransform.X = 0;
    }
}
