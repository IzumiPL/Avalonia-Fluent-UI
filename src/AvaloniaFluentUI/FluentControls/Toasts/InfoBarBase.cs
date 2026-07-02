using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace AvaloniaFluentUI.Controls;

/// <summary>
/// Abstract base class for toast / popup info bar controls.
/// Provides common styled properties (Title, IsClosable, OffsetX/Y, Duration, Content),
/// slide &amp; fade animation helpers, auto-close timer, and close-button wiring.
/// Concrete subclasses only need to declare their own Severity / Position enums
/// and register them as <see cref="StyledProperty{T}"/> fields.
/// </summary>
[TemplatePart(Name = PART_CLOSE_BUTTON, Type = typeof(Button))]
public abstract class InfoBarBase : TemplatedControl
{
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<InfoBarBase, string>(nameof(Title));

    public static readonly StyledProperty<bool> IsClosableProperty =
        AvaloniaProperty.Register<InfoBarBase, bool>(nameof(IsClosable));

    /// <summary>Horizontal pixel offset — animated via <see cref="DoubleTransition"/>.</summary>
    public static readonly StyledProperty<double> OffsetXProperty =
        AvaloniaProperty.Register<InfoBarBase, double>(nameof(OffsetX));

    /// <summary>Vertical pixel offset — animated via <see cref="DoubleTransition"/>.</summary>
    public static readonly StyledProperty<double> OffsetYProperty =
        AvaloniaProperty.Register<InfoBarBase, double>(nameof(OffsetY));

    public static readonly StyledProperty<int> DurationProperty =
        AvaloniaProperty.Register<InfoBarBase, int>(nameof(Duration));

    public static readonly StyledProperty<object?> ContentProperty =
        AvaloniaProperty.Register<InfoBarBase, object?>(nameof(Content));

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public bool IsClosable
    {
        get => GetValue(IsClosableProperty);
        set => SetValue(IsClosableProperty, value);
    }

    public double OffsetX
    {
        get => GetValue(OffsetXProperty);
        set => SetValue(OffsetXProperty, value);
    }

    public double OffsetY
    {
        get => GetValue(OffsetYProperty);
        set => SetValue(OffsetYProperty, value);
    }

    public int Duration
    {
        get => GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    public object? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }
    
    public abstract int PositionValue { get; }
    
    /// <summary>Duration of slide &amp; fade animations in ms.</summary>
    public double AnimationDuration { get; set; } = 300;

    /// <summary>Raised after the close-out animation completes.</summary>
    public event EventHandler? Closed;

    private const string PART_CLOSE_BUTTON = "PART_CloseButton";

    private Button? _closeButton;

    /// <summary>TranslateTransform driven by <see cref="OffsetX"/> / <see cref="OffsetY"/>.</summary>
    private readonly TranslateTransform _translateTransform = new();

    protected InfoBarBase()
    {
        Opacity = 0;
        RenderTransform = _translateTransform;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        StartAutoClose();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        _closeButton?.Click -= OnCloseButtonClick;

        _closeButton = e.NameScope.Find<Button>(PART_CLOSE_BUTTON);

        _closeButton?.Click += OnCloseButtonClick;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == OffsetXProperty)
        {
            _translateTransform.X = OffsetX;
        }
        else if (change.Property == OffsetYProperty)
        {
            _translateTransform.Y = OffsetY;
        }
    }

    private void OnCloseButtonClick(object? sender, RoutedEventArgs e)
    {
        Closed?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Open animation: places the control off-screen at (fromX, fromY), waits for layout,
    /// then transitions to the on-screen position (toX, toY).
    /// </summary>
    public virtual void Run(double fromX, double fromY, double toX, double toY)
    {
        OffsetX = fromX;
        OffsetY = fromY;

        var duration = TimeSpan.FromMilliseconds(AnimationDuration);
        Transitions = new Transitions
        {
            new DoubleTransition
            {
                Property = OffsetXProperty,
                Duration = duration,
                Easing = new CubicEaseOut(),
            },
            new DoubleTransition
            {
                Property = OffsetYProperty,
                Duration = duration,
                Easing = new CubicEaseOut(),
            },
        };

        OffsetX = toX;
        OffsetY = toY;
        Opacity = 1;
    }

    /// <summary>
    /// Reposition the control to a new stacked position (animated via transitions).
    /// </summary>
    public void UpdatePosition(double x, double y)
    {
        OffsetX = x;
        OffsetY = y;
    }

    /// <summary>
    /// Close animation: slides to (x, y) and fades out.
    /// Returns a Task that completes after the animation duration.
    /// </summary>
    public virtual async Task CloseAsync(double x, double y)
    {
        OffsetX = x;
        OffsetY = y;
        await Task.Delay((int)AnimationDuration + 24);
    }

    internal async void StartAutoClose()
    {
        if (Duration >= 0)
        {
            await Task.Delay(Duration);
            Closed?.Invoke(this, EventArgs.Empty);
        }
    }
}
