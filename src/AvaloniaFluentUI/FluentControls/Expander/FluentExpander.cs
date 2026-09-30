using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;

namespace AvaloniaFluentUI.Controls;

/// <summary>
/// 一个带有类似WinUI的扩展/折叠动画的扩展器控件,
/// 支持四个扩展方向.
/// </summary>
[TemplatePart(Name =  PART_EXPANDER_CONTENT, Type = typeof(Border))]
public class FluentExpander : Expander
{
    /// <summary>
    /// Defines the <see cref="EnableContentSizeAnimation"/> property
    /// </summary>
    public static readonly StyledProperty<bool> AnimateContentSizeProperty =
        AvaloniaProperty.Register<FluentExpander, bool>(nameof(EnableContentSizeAnimation), true);

    /// <summary>
    /// 展开/折叠时是否让内容容器的<c>布局尺寸</c>一起做动画（默认开启）。
    /// </summary>
    public bool EnableContentSizeAnimation
    {
        get => GetValue(AnimateContentSizeProperty);
        set => SetValue(AnimateContentSizeProperty, value);
    }

    private Border? _expanderContent;
    private CancellationTokenSource? _cts;

    private double _expanderContentRestingMinHeight;
    private Size _contentSize;
    private readonly TranslateTransform _translateTransform = new();

    private const string PART_EXPANDER_CONTENT = "PART_ExpanderContent";

    /// <summary>模板里给内容容器最小高度的资源键（<c>PART_ExpanderContent</c> 的 <c>MinHeight</c> 引用它）</summary>
    private const string RES_CONTENT_MIN_HEIGHT_RESOURCE = "FluentExpanderContentMinHeight";

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        
        _expanderContent?.SizeChanged -= OnContentSizeChanged;

        _expanderContent = e.NameScope.Find<Border>(PART_EXPANDER_CONTENT);
        _expanderContent?.SizeChanged += OnContentSizeChanged;

        _expanderContentRestingMinHeight = ResolveContentRestingMinHeight(_expanderContent);
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
            ReleaseContentSize();
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
        var token = _cts.Token;

        _expanderContent!.RenderTransform = _translateTransform;
        var wasHidden = !_expanderContent.IsVisible;
        _expanderContent.IsVisible = true;

        if (Parent is ExpanderSettingCard se && se.Presenter != null)
        {
            // ExpanderSettingCard does not use virtualization, so it's safe to use Infinity to measure
            se.Presenter.Measure(Size.Infinity);
            _contentSize = se.Presenter.DesiredSize;
        }
        else
        {
            _contentSize = MeasureContentNaturalSize(Size.Infinity);
            _contentSize = new Size(_contentSize.Width, Math.Max(_contentSize.Height, _expanderContentRestingMinHeight));
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

        var startHeight = wasHidden ? 0d : GetRenderedContentHeight();
        var targetHeight = MeasureContentHeight();

        var translateTask = animation.RunAsync(_expanderContent, token);
        var resizeTask = AnimateContentHeightAsync(startHeight, targetHeight, _expanderContentRestingMinHeight, 333, new KeySpline(0, 0, 0, 1), token);

        ScheduleReleaseContentSize(333, token);

        try
        {
            await Task.WhenAll(translateTask, resizeTask);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        _translateTransform.Y = 0;
    }

    private async void RunCollapseDownUpAnimation(bool down)
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        var token = _cts.Token;

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

        var startHeight = GetRenderedContentHeight();
        if (!double.IsFinite(startHeight) || startHeight <= 0d)
        {
            startHeight = MeasureContentHeight();
        }

        var translateTask = animation.RunAsync(_expanderContent, token);
        var resizeTask = AnimateContentHeightAsync(startHeight, 0d, 0d, 167, new KeySpline(1, 1, 0, 1), token);

        ScheduleReleaseContentSize(167, token);

        try
        {
            await Task.WhenAll(translateTask, resizeTask);
        }
        catch (OperationCanceledException)
        {
            return;
        }

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

    private Task AnimateContentHeightAsync(double from, double to, double toMin, double durationMs, KeySpline keySpline, CancellationToken token)
    {
        if (!EnableContentSizeAnimation || _expanderContent is not { } part)
        {
            return Task.CompletedTask;
        }

        if (!double.IsFinite(from) || !double.IsFinite(to))
        {
            return Task.CompletedTask;
        }

        var fromMin = Math.Min(_expanderContentRestingMinHeight, from);

        var animation = new Animation
        {
            Duration = TimeSpan.FromMilliseconds(durationMs),
            FillMode = FillMode.Forward,
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0),
                    Setters =
                    {
                        new Setter(MaxHeightProperty, from),
                        new Setter(MinHeightProperty, fromMin)
                    }
                },
                new KeyFrame
                {
                    Cue = new Cue(1),
                    Setters =
                    {
                        new Setter(MaxHeightProperty, to),
                        new Setter(MinHeightProperty, toMin)
                    },
                    KeySpline = keySpline
                }
            }
        };

        return animation.RunAsync(part, token);
    }

    /// <summary>
    /// 获取 <c>xaml</c> 里 <c>PART_ExpanderContent</c> 的"静息"最小高度：优先用模板当前给它的值，
    /// 取不到再去查 <c>FluentExpanderContentMinHeight</c> 资源；都不行就返回 0
    /// </summary>
    private double ResolveContentRestingMinHeight(Border? part)
    {
        if (part is not null && double.IsFinite(part.MinHeight) && part.MinHeight > 0d)
        {
            return part.MinHeight;
        }

        if (this.TryFindResource(RES_CONTENT_MIN_HEIGHT_RESOURCE, out var resource) &&
            resource is double value && double.IsFinite(value) && value > 0d)
        {
            return value;
        }

        return 0d;
    }

    /// <summary>
    /// 把内容容器的限高恢复成"不限制"
    /// </summary>
    private void ReleaseContentSize()
    {
        if (!EnableContentSizeAnimation || _expanderContent is not { } part)
        {
            return;
        }

        part.SetValue(MaxHeightProperty, double.PositiveInfinity, BindingPriority.Animation);
    }

    private async void ScheduleReleaseContentSize(double durationMs, CancellationToken token)
    {
        if (!EnableContentSizeAnimation)
            return;

        try
        {
            await Task.Delay((int)durationMs, token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        Dispatcher.UIThread.Post(() =>
        {
            if (token.IsCancellationRequested || _expanderContent is not { } part)
            {
                return;
            }

            part.SetValue(MaxHeightProperty, double.PositiveInfinity, BindingPriority.Animation);
            part.SetValue(MinHeightProperty, _expanderContentRestingMinHeight, BindingPriority.Animation);
        });
    }

    /// <summary>内容容器当前渲染出来的高度（布局还没跑过时返回 <see cref="double.NaN"/>）</summary>
    private double GetRenderedContentHeight()
    {
        var height = _expanderContent?.Bounds.Height ?? double.NaN;
        return double.IsFinite(height) && height >= 0d ? height : double.NaN;
    }

    private Size MeasureContentNaturalSize(Size availableSize)
    {
        if (_expanderContent is not { } part || part.Child is not { } child)
        {
            return default;
        }

        var border = part.BorderThickness;
        var innerWidth = Math.Max(0d, availableSize.Width - border.Left - border.Right);
        var innerHeight = Math.Max(0d, availableSize.Height - border.Top - border.Bottom);

        child.Measure(new Size(innerWidth, innerHeight)); var size = child.DesiredSize;

        return new Size(size.Width + border.Left + border.Right, size.Height + border.Top + border.Bottom);
    }

    private double MeasureContentHeight()
    {
        if (_expanderContent is not { } part)
        {
            return double.PositiveInfinity;
        }

        var width = part.Bounds.Width;
        if (!double.IsFinite(width) || width <= 0d)
        {
            width = Bounds.Width;
        }

        if (!double.IsFinite(width) || width <= 0d)
        {
            return double.PositiveInfinity;
        }

        var constraint = new Size(width, double.PositiveInfinity);
        var height = MeasureContentNaturalSize(constraint).Height;

        if (height <= 0d)
        {
            part.Child?.InvalidateMeasure();
            height = MeasureContentNaturalSize(constraint).Height;
        }

        if (height <= 0d)
        {
            return double.PositiveInfinity;
        }

        return Math.Max(height, _expanderContentRestingMinHeight);
    }
}
