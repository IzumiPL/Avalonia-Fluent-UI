
using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using AvaloniaFluentUI.Core;

namespace AvaloniaFluentUI.Controls;

[PseudoClasses(PC_OPEN, PC_LEFT, PC_TOP, PC_RIGHT, PC_BOTTOM)]
[TemplatePart(Name = PART_CONTENT_HOST,     Type = typeof(Border))]
[TemplatePart(Name = PART_CLOSE_BUTTON,     Type = typeof(ToolButton))]
[TemplatePart(Name = PART_HIT_TEST_AREA,    Type = typeof(Border))]
public class PopupDrawer : ContentControl
{
    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<PopupDrawer, bool>(nameof(IsOpen), defaultBindingMode: BindingMode.TwoWay, coerce: CoerceIsOpen);

    public static readonly StyledProperty<PopupDrawerPosition> PositionProperty =
        AvaloniaProperty.Register<PopupDrawer, PopupDrawerPosition>(nameof(Position));

    public static readonly StyledProperty<PopupDrawerMode> ModeProperty =
        AvaloniaProperty.Register<PopupDrawer, PopupDrawerMode>(nameof(Mode), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<double> DrawerWidthProperty =
        AvaloniaProperty.Register<PopupDrawer, double>(nameof(DrawerWidth), 328);

    public static readonly StyledProperty<double> DrawerHeightProperty =
        AvaloniaProperty.Register<PopupDrawer, double>(nameof(DrawerHeight), double.NaN);

    public static readonly StyledProperty<bool> IsLightDismissEnabledProperty =
        AvaloniaProperty.Register<PopupDrawer, bool>(nameof(IsLightDismissEnabled), true);

    public static readonly StyledProperty<bool> CloseButtonIsVisibleProperty =
        AvaloniaProperty.Register<PopupDrawer, bool>(nameof(CloseButtonIsVisible), true);

    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public PopupDrawerPosition Position
    {
        get => GetValue(PositionProperty);
        set => SetValue(PositionProperty, value);
    }

    public PopupDrawerMode Mode
    {
        get => GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }

    public double DrawerWidth
    {
        get => GetValue(DrawerWidthProperty);
        set => SetValue(DrawerWidthProperty, value);
    }

    public double DrawerHeight
    {
        get => GetValue(DrawerHeightProperty);
        set => SetValue(DrawerHeightProperty, value);
    }

    public bool IsLightDismissEnabled
    {
        get => GetValue(IsLightDismissEnabledProperty);
        set => SetValue(IsLightDismissEnabledProperty, value);
    }

    public bool CloseButtonIsVisible
    {
        get => GetValue(CloseButtonIsVisibleProperty);
        set => SetValue(CloseButtonIsVisibleProperty, value);
    }

    public bool IsRunning { get; private set; }

    private const string PART_CONTENT_HOST = "PART_ContentHost";
    private const string PART_CLOSE_BUTTON = "PART_CloseButton";
    private const string PART_HIT_TEST_AREA = "PART_HitTestArea";

    private const string PC_OPEN = ":open";
    private const string PC_LEFT = ":left";
    private const string PC_TOP = ":top";
    private const string PC_RIGHT = ":right";
    private const string PC_BOTTOM = ":bottom";

    private Border? _contentHost;
    private Border? _hitTestArea;
    private ToolButton? _closeButton;

    private readonly TranslateTransform _transform = new();

    private CancellationTokenSource? _animationCts;

    private TopLevel? _owner;
    private OverlayLayer? _overlayLayer;

    private Panel? _inlineParent;

    private double _inlineCanvasLeft;
    private double _inlineCanvasTop;

    private bool _isInlineLayoutSaved;
    private bool _isOverlayAttached;

    private bool _isWatchingInlineParent;
    private bool _isClosingWithoutAnimation;

    private const double OpenDuration = 333;
    private const double CloseDuration = 167;

    public void Show()
    {
        SetCurrentValue(IsOpenProperty, true);
    }

    public void Hide()
    {
        SetCurrentValue(IsOpenProperty, false);
    }

    public void Toggle()
    {
        SetCurrentValue(IsOpenProperty, !IsOpen);
    }

    private static bool CoerceIsOpen(AvaloniaObject sender, bool value)
    {
        if (sender is PopupDrawer drawer && drawer.IsRunning)
            return drawer.IsOpen;

        return value;
    }

    private IDisposable? _rootBoundsWatcher;

    protected override Size MeasureOverride(Size availableSize)
    {
        var size = base.MeasureOverride(availableSize);
        if (TopLevel.GetTopLevel(this) is TopLevel tl)
            return tl.ClientSize;
        else if (TopLevel.GetTopLevel(this) is Control c)
            return c.Bounds.Size;

        return size;
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        if (TopLevel.GetTopLevel(this) is Control c)
            _rootBoundsWatcher = c.GetObservable(BoundsProperty).Subscribe(_ => InvalidateVisual());
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _rootBoundsWatcher?.Dispose();
        _rootBoundsWatcher = null;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _closeButton?.Click -= OnCloseButtonClick;
        _hitTestArea?.PointerPressed -= OnHitTestAreaPointerPressed;

        _contentHost = e.NameScope.Find<Border>(PART_CONTENT_HOST);
        _closeButton = e.NameScope.Find<ToolButton>(PART_CLOSE_BUTTON);
        _hitTestArea = e.NameScope.Find<Border>(PART_HIT_TEST_AREA);

        if (_contentHost != null)
        {
            _contentHost.RenderTransform = _transform;
            _contentHost.IsVisible = IsOpen;
        }

        if (_hitTestArea != null)
        {
            _hitTestArea.PointerPressed += OnHitTestAreaPointerPressed;
            _hitTestArea.IsVisible = IsOpen;
        }

        _closeButton?.Click += OnCloseButtonClick;

        UpdatePseudoClasses();

        if (IsOpen)
            _ = RunPopupDrawerAnimationAsync();
        else
            ResetPosition();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsOpenProperty)
        {
            UpdatePseudoClasses();
            _ = RunPopupDrawerAnimationAsync();
        }
        else if (change.Property == ModeProperty)
        {
            UpdateAttachment();
        }
        else if (change.Property == PositionProperty)
        {
            UpdatePseudoClasses();

            if (!IsOpen)
                ResetPosition();
        }
        else if (change.Property == DrawerWidthProperty ||
                 change.Property == DrawerHeightProperty)
        {
            if (!IsOpen)
                ResetPosition();
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(PC_OPEN, IsOpen);
        PseudoClasses.Set(PC_LEFT, Position == PopupDrawerPosition.Left);
        PseudoClasses.Set(PC_RIGHT, Position == PopupDrawerPosition.Right);
        PseudoClasses.Set(PC_TOP, Position == PopupDrawerPosition.Top);
        PseudoClasses.Set(PC_BOTTOM, Position == PopupDrawerPosition.Bottom);
    }

    private double GetDistance()
    {
        if (Position is PopupDrawerPosition.Left or PopupDrawerPosition.Right)
            return Math.Max(0, DrawerWidth);

        if (!double.IsNaN(DrawerHeight) && !double.IsInfinity(DrawerHeight))
            return Math.Max(0, DrawerHeight);

        if (_contentHost is { Bounds.Height: > 0 })
            return _contentHost.Bounds.Height;

        if (Bounds.Height > 0)
            return Bounds.Height;

        return _owner?.ClientSize.Height ?? 0;
    }

    private double GetHiddenOffset()
    {
        var distance = GetDistance();

        return Position switch
        {
            PopupDrawerPosition.Left => -distance,
            PopupDrawerPosition.Right => distance,
            PopupDrawerPosition.Top => -distance,
            PopupDrawerPosition.Bottom => distance,
            _ => 0
        };
    }

    private void ResetPosition()
    {
        if (_contentHost == null)
            return;

        _transform.X = 0;
        _transform.Y = 0;

        switch (Position)
        {
            case PopupDrawerPosition.Left:
            case PopupDrawerPosition.Right:
                _transform.X = GetHiddenOffset();
                break;

            case PopupDrawerPosition.Top:
            case PopupDrawerPosition.Bottom:
                _transform.Y = GetHiddenOffset();
                break;
        }
    }

    private void SaveInlineLayout()
    {
        if (_isInlineLayoutSaved)
            return;

        if (Parent is not Panel parent)
            throw new InvalidOperationException("Inline 模式要求 PopupDrawer 的直接父容器为 Panel");

        _inlineParent = parent;

        _inlineCanvasLeft = Canvas.GetLeft(this);
        _inlineCanvasTop = Canvas.GetTop(this);

        _isInlineLayoutSaved = true;

        if (_isWatchingInlineParent)
            return;

        _inlineParent.AttachedToVisualTree += OnInlineParentAttachedToVisualTree;
        _inlineParent.DetachedFromVisualTree += OnInlineParentDetachedFromVisualTree;

        _isWatchingInlineParent = true;
    }

    private void OnInlineParentAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (Parent == null && _inlineParent != null && !_inlineParent.Children.Contains(this))
        {
            _inlineParent.Children.Add(this);
            Canvas.SetLeft(this, _inlineCanvasLeft);
            Canvas.SetTop(this, _inlineCanvasTop);
        }

        UpdateAttachment();
    }

    private void OnInlineParentDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        CloseImmediately();
    }

    /// <summary>
    ///     同步挂载状态到当前 <see cref="Mode" />，用于运行期切换显示模式时在 overlay 与原始布局之间迁移。
    /// </summary>
    private void UpdateAttachment()
    {
        if (!IsOpen || _contentHost == null)
            return;

        if (Mode == PopupDrawerMode.Overlay)
        {
            if (TopLevel.GetTopLevel(this) == null)
                AttachInline();

            AttachOverlay();
        }
        else
        {
            AttachInline();
        }
    }

    private void AttachOverlay()
    {
        if (_isOverlayAttached)
            return;

        var owner = TopLevel.GetTopLevel(this);

        if (owner == null)
            return;

        var overlayLayer = OverlayLayer.GetOverlayLayer(this);

        if (overlayLayer == null)
            throw new InvalidOperationException("当前 TopLevel 不存在 OverlayLayer");

        _owner = owner;
        _overlayLayer = overlayLayer;

        SaveInlineLayout();

        if (Parent is Panel parent)
            parent.Children.Remove(this);

        Canvas.SetLeft(this, 0);
        Canvas.SetTop(this, 0);

        if (!_overlayLayer.Children.Contains(this))
            _overlayLayer.Children.Add(this);

        _isOverlayAttached = true;
    }

    private void AttachInline()
    {
        if (!_isInlineLayoutSaved)
            SaveInlineLayout();

        if (_isOverlayAttached)
        {
            _overlayLayer?.Children.Remove(this);
            _isOverlayAttached = false;
        }

        if (_inlineParent == null)
            throw new InvalidOperationException("无法获取 PopupDrawer 的 Inline 父容器");

        if (Parent == null)
            _inlineParent.Children.Add(this);

        Canvas.SetLeft(this, _inlineCanvasLeft);
        Canvas.SetTop(this, _inlineCanvasTop);
    }

    private void DetachOverlay()
    {
        if (!_isOverlayAttached)
            return;

        _overlayLayer?.Children.Remove(this);

        _isOverlayAttached = false;
    }

    /// <summary>
    ///     立即释放 overlay 挂载并把抽屉置为关闭状态，不再播放动画 (控件可能已脱离可视树)。
    /// </summary>
    private void CloseImmediately()
    {
        _animationCts?.Cancel();
        _animationCts = null;
        IsRunning = false;

        DetachOverlay();

        _overlayLayer = null;
        _owner = null;

        if (IsOpen)
        {
            _isClosingWithoutAnimation = true;

            try
            {
                SetCurrentValue(IsOpenProperty, false);
            }
            finally
            {
                _isClosingWithoutAnimation = false;
            }

            UpdatePseudoClasses();
        }

        ResetPosition();

        if (_contentHost != null)
            _contentHost.IsVisible = false;

        if (_hitTestArea != null)
            _hitTestArea.IsVisible = false;
    }

    private async Task RunPopupDrawerAnimationAsync()
    {
        if (_contentHost == null)
            return;

        if (_isClosingWithoutAnimation)
            return;

        var currentX = _transform.X;
        var currentY = _transform.Y;

        _animationCts?.Cancel();

        var cts = new CancellationTokenSource();
        _animationCts = cts;

        IsRunning = true;

        try
        {
            if (IsOpen)
            {
                UpdateAttachment();

                _contentHost.IsVisible = true;

                if (_hitTestArea != null)
                    _hitTestArea.IsVisible = true;

                if (Math.Abs(currentX) < 0.001 && Math.Abs(currentY) < 0.001)
                {
                    ResetPosition();
                }
                else
                {
                    _transform.X = currentX;
                    _transform.Y = currentY;
                }
            }
            else
            {
                _transform.X = currentX;
                _transform.Y = currentY;
            }

            var horizontal = Position is
                PopupDrawerPosition.Left or
                PopupDrawerPosition.Right;

            var property = horizontal
                ? TranslateTransform.XProperty
                : TranslateTransform.YProperty;

            var from = horizontal ? _transform.X : _transform.Y;
            var to = IsOpen ? 0 : GetHiddenOffset();

            var animation = new Animation
            {
                Duration = TimeSpan.FromMilliseconds(
                    IsOpen ? OpenDuration : CloseDuration),

                FillMode = FillMode.Forward,

                Easing = IsOpen
                    ? new CubicEaseOut()
                    : new CubicEaseIn(),

                Children =
                {
                    new KeyFrame
                    {
                        Cue = new Cue(0),
                        Setters =
                        {
                            new Setter(property, from)
                        }
                    },
                    new KeyFrame
                    {
                        Cue = new Cue(1),
                        Setters =
                        {
                            new Setter(property, to)
                        }
                    }
                }
            };

            await animation.RunAsync(_contentHost, cts.Token);

            if (cts.IsCancellationRequested)
                return;

            if (horizontal)
                _transform.X = to;
            else
                _transform.Y = to;

            if (!IsOpen)
            {
                _contentHost.IsVisible = false;

                if (_hitTestArea != null)
                    _hitTestArea.IsVisible = false;

                DetachOverlay();

                AttachInline();
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch
        {
            if (ReferenceEquals(_animationCts, cts))
            {
                if (_contentHost != null)
                    _contentHost.IsVisible = IsOpen;

                if (_hitTestArea != null)
                    _hitTestArea.IsVisible = IsOpen;
            }

            throw;
        }
        finally
        {
            if (ReferenceEquals(_animationCts, cts))
            {
                _animationCts = null;
                IsRunning = false;
            }

            cts.Dispose();
        }
    }

    private void OnCloseButtonClick(object? sender, RoutedEventArgs e)
    {
        Hide();
    }

    private void OnHitTestAreaPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!IsOpen || !IsLightDismissEnabled)
            return;

        Hide();
        e.Handled = true;
    }
}
