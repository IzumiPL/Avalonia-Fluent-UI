using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Threading;
using AvaloniaFluentUI.Core;

namespace AvaloniaFluentUI.Controls;

[TemplatePart(Name = PART_MIN_THUMB,        Type = typeof(Thumb))]
[TemplatePart(Name = PART_MAX_THUMB,        Type = typeof(Thumb))]
[TemplatePart(Name = PART_POPUP_MIN,        Type = typeof(Popup))]
[TemplatePart(Name = PART_POPUP_MAX,        Type = typeof(Popup))]
[TemplatePart(Name = CONTAINER_CANVAS,      Type = typeof(Canvas))]
[TemplatePart(Name = PART_ACTIVE_RECTANGLE, Type = typeof(Rectangle))]
[TemplatePart(Name = PART_TOOLTIP_TEXT_MIN, Type = typeof(TextBlock))]
[TemplatePart(Name = PART_TOOLTIP_TEXT_MAX, Type = typeof(TextBlock))]
public class RangeSlider : TemplatedControl
{
    /// <summary>
    /// Defines the <see cref="Minimum"/> property
    /// </summary>
    public static readonly StyledProperty<double> MinimumProperty = 
        RangeBase.MinimumProperty.AddOwner<RangeSlider>(new StyledPropertyMetadata<double>(0d));

    /// <summary>
    /// Defines the <see cref="Maximum"/> property
    /// </summary>
    public static readonly StyledProperty<double> MaximumProperty = 
        RangeBase.MaximumProperty.AddOwner<RangeSlider>(new StyledPropertyMetadata<double>(100d));

    /// <summary>
    /// Defines the <see cref="RangeStart"/> property
    /// </summary>
    public static readonly StyledProperty<double> RangeStartProperty = 
        AvaloniaProperty.Register<RangeSlider, double>(nameof(RangeStart), defaultValue: 0, defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// Defines the <see cref="RangeEnd"/> property
    /// </summary>
    public static readonly StyledProperty<double> RangeEndProperty = 
        AvaloniaProperty.Register<RangeSlider, double>(nameof(RangeEnd), defaultValue: 100, defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// Defines the <see cref="StepFrequency"/> property
    /// </summary>
    public static readonly StyledProperty<double> StepFrequencyProperty = 
        AvaloniaProperty.Register<RangeSlider, double>(nameof(StepFrequency), defaultValue: 1);

    /// <summary>
    /// Defines the <see cref="ToolTipStringFormat"/> property
    /// </summary>
    public static readonly StyledProperty<string> ToolTipStringFormatProperty =
        AvaloniaProperty.Register<RangeSlider, string>(nameof(ToolTipStringFormat), "0.##");

    /// <summary>
    /// Defines the <see cref="MinimumRange"/> property
    /// </summary>
    public static readonly StyledProperty<double> MinimumRangeProperty = 
        AvaloniaProperty.Register<RangeSlider, double>(nameof(MinimumRange), defaultValue: 0d);

    /// <summary>
    /// Defines the <see cref="ShowValueToolTip"/> property
    /// </summary>
    public static readonly StyledProperty<bool> ShowValueToolTipProperty = 
        AvaloniaProperty.Register<RangeSlider, bool>(nameof(ShowValueToolTip), defaultValue: true);

    /// <summary>
    /// Gets or sets the minimum allowed value for the RangeSlider
    /// </summary>
    public double Minimum
    {
        get => GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum allowed value for the RangeSlider
    /// </summary>
    public double Maximum
    {
        get => GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    /// <summary>
    /// Gets or sets the start of the selected range
    /// </summary>
    public double RangeStart
    {
        get => GetValue(RangeStartProperty);
        set => SetValue(RangeStartProperty, value);
    }

    /// <summary>
    /// Gets or sets the end of the selected range
    /// </summary>
    public double RangeEnd
    {
        get => GetValue(RangeEndProperty);
        set => SetValue(RangeEndProperty, value);
    }

    /// <summary>
    /// Gets or sets the frequency of ticks when dragging the slider
    /// </summary>
    public double StepFrequency
    {
        get => GetValue(StepFrequencyProperty);
        set => SetValue(StepFrequencyProperty, value);
    }

    /// <summary>
    /// Gets or sets the string format used in the value ToolTip when dragging
    /// </summary>
    public string ToolTipStringFormat
    {
        get => GetValue(ToolTipStringFormatProperty);
        set => SetValue(ToolTipStringFormatProperty, value);
    }

    /// <summary>
    /// Gets or sets the smallest acceptable range between <see cref="RangeStart"/> and <see cref="RangeEnd"/>
    /// when dragging the thumb
    /// </summary>
    /// <remarks>
    /// Use this property to set a minimum distance (in data units) the slider thumbs can get during a drag operation
    /// to prevent them from overlapping. NOTE: This property does NOT have any effect if the RangeStart or RangeEnd
    /// is set programmatically, i.e., Start = 30, End = 50, MinimumRange=15, you cannot drag the RangeStart thumb to 40,
    /// but you can still programmatically set RangeStart to 40.
    /// </remarks>
    public double MinimumRange
    {
        get => GetValue(MinimumRangeProperty);
        set => SetValue(MinimumRangeProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the Value ToolTip is shown when dragging a thumb
    /// </summary>
    public bool ShowValueToolTip
    {
        get => GetValue(ShowValueToolTipProperty);
        set => SetValue(ShowValueToolTipProperty, value);
    }
    
    // Internal for UnitTests
    internal double DragWidth => (_containerCanvas != null && _maxThumb != null) ? _containerCanvas.Bounds.Width - _maxThumb.Bounds.Width : 0;

    /// <summary>
    /// Fired when a thumb drag begins
    /// </summary>
    public event EventHandler<VectorEventArgs>? ThumbDragStarted;

    /// <summary>
    /// Fired when a thumb drag completes
    /// </summary>
    public event EventHandler<VectorEventArgs>? ThumbDragCompleted;

    /// <summary>
    /// Fired when either RangeStart or RangeEnd is changed
    /// </summary>
    public event EventHandler<RangeChangedEventArgs>? ValueChanged;
    
    private Rectangle? _activeRectangle;
    private Thumb? _minThumb;
    private Thumb? _maxThumb;
    private Canvas? _containerCanvas;
    private double _oldValue;
    private bool _valuesAssigned;
    private bool _minSet;
    private bool _maxSet;
    private bool _pointerManipulatingMin;
    private bool _pointerManipulatingMax;
    private bool _pointerManipulatingBoth;
    private double _absolutePosition;
    private Popup? _minPopup;
    private Popup? _maxPopup;
    private TextBlock? _minToolTipText;
    private TextBlock? _maxToolTipText;
    private const double Epsilon = 0.01;
    private bool _isDraggingStart;
    private bool _isDraggingEnd;
    private bool _isDrag;
    private readonly DispatcherTimer _closeToolTipTimer = new DispatcherTimer();

    private const string PART_ACTIVE_RECTANGLE = "PART_ActiveRectangle";
    private const string PART_MIN_THUMB = "PART_MinThumb";
    private const string PART_MAX_THUMB = "PART_MaxThumb";
    private const string CONTAINER_CANVAS = "ContainerCanvas";
    private const string PART_POPUP_MIN = "PART_PopupMin";
    private const string PART_POPUP_MAX = "PART_PopupMax";
    private const string PART_TOOLTIP_TEXT_MIN = "PART_ToolTipTextMin";
    private const string PART_TOOLTIP_TEXT_MAX = "PART_ToolTipTextMax";

    public RangeSlider()
    {
        _closeToolTipTimer.Interval = TimeSpan.FromMilliseconds(200);
        _closeToolTipTimer.Tick += ClosePopup;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == RangeStartProperty)
        {
            _minSet = true;

            if (!_valuesAssigned)
                return;

            var newV = change.GetNewValue<double>();
            RangeMinToStepFrequency();

            if (_valuesAssigned)
            {
                if (newV < Minimum)
                    RangeStart = Minimum;
                else if (newV > Maximum)
                    RangeStart = Maximum;

                SyncActiveRectangle();

                if (newV > RangeEnd)
                    RangeEnd = newV;
            }

            SyncThumbs();

            if (!_isDraggingEnd && !_isDraggingStart)
            {
                OnValueChanged(new RangeChangedEventArgs(change.GetOldValue<double>(), newV, RangeSelectorProperty.RangeStartValue));
            }
        }
        else if (change.Property == RangeEndProperty)
        {
            _maxSet = true;

            if (!_valuesAssigned)
                return;

            var newV = change.GetNewValue<double>();
            RangeMaxToStepFrequency();

            if (_valuesAssigned)
            {
                if (newV < Minimum)
                    RangeEnd = Minimum;
                else if (newV > Maximum)
                    RangeEnd = Maximum;

                SyncActiveRectangle();

                if (newV < RangeStart)
                    RangeStart = newV;
            }

            SyncThumbs();

            if (!_isDraggingEnd && !_isDraggingStart)
            {
                OnValueChanged(new RangeChangedEventArgs(change.GetOldValue<double>(), newV, RangeSelectorProperty.RangeEndValue));
            }
        }
        else if (change.Property == MinimumProperty)
        {
            if (!_valuesAssigned)
                return;

            var (oldV, newV) = change.GetOldAndNewValue<double>();

            if (Maximum < newV)
                Maximum = newV + Epsilon;

            if (RangeStart < newV)
                RangeStart = newV;

            if (RangeEnd < newV)
                RangeEnd = newV;

            if (!newV.Equals(oldV))
                SyncThumbs();
        }
        else if (change.Property == MaximumProperty)
        {
            if (!_valuesAssigned)
                return;

            var (oldV, newV) = change.GetOldAndNewValue<double>();

            if (Minimum > newV)
                Maximum = newV + Epsilon;

            if (RangeEnd > newV)
                RangeEnd = newV;

            if (RangeStart > newV)
                RangeStart = newV;

            if (!newV.Equals(oldV))
                SyncThumbs();
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        UnsubscribeEvents();
        base.OnApplyTemplate(e);

        VerifyValues();
        _valuesAssigned = true;

        _activeRectangle = e.NameScope.Get<Rectangle>(PART_ACTIVE_RECTANGLE);
        _minThumb = e.NameScope.Get<Thumb>(PART_MIN_THUMB);
        _maxThumb = e.NameScope.Get<Thumb>(PART_MAX_THUMB);
        _containerCanvas = e.NameScope.Get<Canvas>(CONTAINER_CANVAS);
        _minPopup = e.NameScope.Find<Popup>(PART_POPUP_MIN);
        _maxPopup = e.NameScope.Find<Popup>(PART_POPUP_MAX);
        _minToolTipText = e.NameScope.Find<TextBlock>(PART_TOOLTIP_TEXT_MIN);
        _maxToolTipText = e.NameScope.Find<TextBlock>(PART_TOOLTIP_TEXT_MAX);

        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        if (_minThumb != null)
        {
            _minThumb.DragCompleted += HandleThumbDragCompleted;
            _minThumb.DragDelta += MinThumbDragDelta;
            _minThumb.DragStarted += MinThumbDragStarted;
            _minThumb.KeyDown += MinThumbKeyDown;
            _minThumb.KeyUp += ThumbKeyUp;
        }

        if (_maxThumb != null)
        {
            _maxThumb.DragCompleted += HandleThumbDragCompleted;
            _maxThumb.DragDelta += MaxThumbDragDelta;
            _maxThumb.DragStarted += MaxThumbDragStarted;
            _maxThumb.KeyDown += MaxThumbKeyDown;
            _maxThumb.KeyUp += ThumbKeyUp;
        }

        if (_containerCanvas != null)
        {
            _containerCanvas.SizeChanged += ContainerCanvasSizeChanged;
            _containerCanvas.PointerPressed += ContainerCanvasPointerPressed;
            _containerCanvas.PointerMoved += ContainerCanvasPointerMoved;
            _containerCanvas.PointerReleased += ContainerCanvasPointerReleased;
            _containerCanvas.PointerExited += ContainerCanvasPointerExited;
        }
    }

    private void UnsubscribeEvents()
    {
        if (_minThumb != null)
        {
            _minThumb.DragCompleted -= HandleThumbDragCompleted;
            _minThumb.DragDelta -= MinThumbDragDelta;
            _minThumb.DragStarted -= MinThumbDragStarted;
            _minThumb.KeyDown -= MinThumbKeyDown;
            _minThumb.KeyUp -= ThumbKeyUp;
        }

        if (_maxThumb != null)
        {
            _maxThumb.DragCompleted -= HandleThumbDragCompleted;
            _maxThumb.DragDelta -= MaxThumbDragDelta;
            _maxThumb.DragStarted -= MaxThumbDragStarted;
            _maxThumb.KeyDown -= MaxThumbKeyDown;
            _maxThumb.KeyUp -= ThumbKeyUp;
        }
        
        if (_containerCanvas != null)
        { 
            _containerCanvas.SizeChanged -= ContainerCanvasSizeChanged;
            _containerCanvas.PointerPressed -= ContainerCanvasPointerPressed;
            _containerCanvas.PointerMoved -= ContainerCanvasPointerMoved;
            _containerCanvas.PointerReleased -= ContainerCanvasPointerReleased;
            _containerCanvas.PointerExited -= ContainerCanvasPointerExited;
        }
        
    }

    protected virtual void OnThumbDragStarted(VectorEventArgs e)
    {
        ThumbDragStarted?.Invoke(this, e);
    }

    protected virtual void OnThumbDragCompleted(VectorEventArgs e)
    {
        ThumbDragCompleted?.Invoke(this, e);
    }

    protected virtual void OnValueChanged(RangeChangedEventArgs e)
    {
        ValueChanged?.Invoke(this, e);
    }

    private void MinThumbDragDelta(object? sender, VectorEventArgs e)
    {
        if (_minThumb == null) { return; }

        _absolutePosition += e.Vector.X;

        var oldStart = RangeStart;
        var newStart = DragThumb(_minThumb, 0, DragWidth, _absolutePosition);

        var limit = RangeEnd - MinimumRange;
        if (newStart > limit)
        {
            RangeEnd += newStart - oldStart;
            newStart -= newStart - limit;
            RangeStart = newStart;
        }
        else
        {
            RangeStart = newStart;
        }

        UpdateToolTipTexts();
    }

    private void MaxThumbDragDelta(object? sender, VectorEventArgs e)
    {
        if (_maxThumb == null) { return; }
        
        _absolutePosition += e.Vector.X;

        var oldEnd = RangeEnd;
        var newEnd = DragThumb(_maxThumb, 0, DragWidth, _absolutePosition);

        var limit = RangeStart + MinimumRange;
        if (newEnd < limit)
        {
            RangeStart -= oldEnd - newEnd;
            newEnd -= newEnd - limit;
            RangeEnd = newEnd;
        }
        else
        {
            RangeEnd = newEnd;
        }

        UpdateToolTipTexts();
    }

    private void MinThumbDragStarted(object? sender, VectorEventArgs e)
    {
        _isDraggingStart = true;
        OnThumbDragStarted(e);
        HandleThumbDragStarted(_minThumb);
    }

    private void MaxThumbDragStarted(object? sender, VectorEventArgs e)
    {
        _isDraggingEnd = true;
        OnThumbDragStarted(e);
        HandleThumbDragStarted(_maxThumb);
    }

    private void HandleThumbDragCompleted(object? sender, VectorEventArgs e)
    {
        if (sender != null)
        {
            _isDraggingStart = _isDraggingEnd = false;
            _isDrag = false;
            OnThumbDragCompleted(e);
            OnValueChanged(sender.Equals(_minThumb) ?
                new RangeChangedEventArgs(_oldValue, RangeStart, RangeSelectorProperty.RangeStartValue) :
                new RangeChangedEventArgs(_oldValue, RangeEnd, RangeSelectorProperty.RangeEndValue));
            SyncThumbs();

            RestartCloseToolTipTimer();
        }
    }

    private double DragThumb(Thumb thumb, double min, double max, double nextPos)
    {
        nextPos = Math.Max(min, nextPos);
        nextPos = Math.Min(max, nextPos);

        Canvas.SetLeft(thumb, nextPos);

        return Minimum + ((nextPos / DragWidth) * (Maximum - Minimum));
    }

    private void HandleThumbDragStarted(Thumb? thumb)
    {
        if (thumb == null) { return; }
        
        var useMin = thumb == _minThumb;
        var otherThumb = useMin ? _maxThumb : _minThumb;

        _absolutePosition = Canvas.GetLeft(thumb);
        thumb.ZIndex = 10;
        otherThumb?.ZIndex = 0;
        _oldValue = useMin ? RangeStart : RangeEnd;

        _isDrag = true;
        ShowToolTips();
    }

    private void MinThumbKeyDown(object? sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Left:
                RangeStart -= StepFrequency;
                _isDrag = true;
                ShowToolTips();
                e.Handled = true;
                break;

            case Key.Right:
                RangeStart += StepFrequency;
                _isDrag = true;
                ShowToolTips();
                e.Handled = true;
                break;
        }
    }

    private void MaxThumbKeyDown(object? sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Left:
                RangeEnd -= StepFrequency;
                _isDrag = true;
                ShowToolTips();
                e.Handled = true;
                break;
            case Key.Right:
                RangeEnd += StepFrequency;
                _isDrag = true;
                ShowToolTips();
                e.Handled = true;
                break;
        }
    }

    private void ThumbKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Left || e.Key == Key.Right)
        {
            _isDrag = false; 
            RestartCloseToolTipTimer();
            
            e.Handled = true;
        }
    }

    private void ContainerCanvasPointerExited(object? sender, PointerEventArgs e)
    {
        if (_containerCanvas == null) { return; }
        
        var position = e.GetCurrentPoint(_containerCanvas).Position;

        // Bug in Avalonia.InputElement.PointerExited // https://github.com/avaloniaui/avalonia/issues/20520
        if (position.X >= _containerCanvas.Bounds.Left && position.X <= _containerCanvas.Bounds.Right && position.Y >= _containerCanvas.Bounds.Top && position.Y <= _containerCanvas.Bounds.Bottom)
            return;

        var normalizedPosition = ((position.X / DragWidth) * (Maximum - Minimum)) + Minimum;

        if (_pointerManipulatingMin)
        {
            _pointerManipulatingMin = false;
            _containerCanvas.IsHitTestVisible = true;
            OnValueChanged(new RangeChangedEventArgs(RangeStart, normalizedPosition, RangeSelectorProperty.RangeStartValue));
        }
        else if (_pointerManipulatingMax)
        {
            _pointerManipulatingMax = false;
            _containerCanvas.IsHitTestVisible = true;
            OnValueChanged(new RangeChangedEventArgs(RangeEnd, normalizedPosition, RangeSelectorProperty.RangeEndValue));
        }
    }

    private void ContainerCanvasPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _pointerManipulatingBoth = false;
        var position = e.GetCurrentPoint(_containerCanvas).Position.X;
        var normalizedPosition = ((position / DragWidth) * (Maximum - Minimum)) + Minimum;

        _isDrag = false;
        RestartCloseToolTipTimer();

        if (_pointerManipulatingMin)
        {
            _pointerManipulatingMin = false;
            _containerCanvas?.IsHitTestVisible = true;
            OnValueChanged(new RangeChangedEventArgs(RangeStart, normalizedPosition, RangeSelectorProperty.RangeStartValue));
        }
        else if (_pointerManipulatingMax)
        {
            _pointerManipulatingMax = false;
            _containerCanvas?.IsHitTestVisible = true;
            OnValueChanged(new RangeChangedEventArgs(RangeEnd, normalizedPosition, RangeSelectorProperty.RangeEndValue));
        }

        SyncThumbs();
    }

    private void ContainerCanvasPointerMoved(object? sender, PointerEventArgs e)
    {
        var position = e.GetCurrentPoint(_containerCanvas).Position.X;
        if (_pointerManipulatingBoth)
        {
            var max = Maximum;
            var min = Minimum;
            var dragDelta = position - _absolutePosition;
            var delta = ((dragDelta / DragWidth) * (max - min));
            if (Math.Abs(delta) < StepFrequency)
                return;
            var rs = RangeStart;
            var re = RangeEnd;
            
            if (delta > 0)
            {
                if (MathHelpers.IsClose(re, max))
                    return;

                // Drag delta is too large, constrain it back
                if (re + delta > max)
                    delta = max - re;
            }
            else if (delta < 0)
            {
                if (MathHelpers.IsClose(rs, min))
                    return;

                if (rs + delta < min)
                    delta = min - rs;
            }


            RangeStart += delta;
            RangeEnd += delta;
            _absolutePosition = position;
            UpdateToolTipTexts();
            return;
        }

        var normalizedPosition = ((position / DragWidth) * (Maximum - Minimum)) + Minimum;
        if (_minThumb == null || _maxThumb == null) { return; }

        if (_pointerManipulatingMin && normalizedPosition < RangeEnd)
        {
            RangeStart = DragThumb(_minThumb, 0, Canvas.GetLeft(_maxThumb), position);
            UpdateToolTipTexts();
        }
        else if (_pointerManipulatingMax && normalizedPosition > RangeStart)
        {
            RangeEnd = DragThumb(_maxThumb, Canvas.GetLeft(_minThumb), DragWidth, position);
            UpdateToolTipTexts();
        }
    }

    private void ContainerCanvasPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) { return; }
        
        var position = e.GetCurrentPoint(_containerCanvas).Position.X;

        var mods = Application.Current?.PlatformSettings?.HotkeyConfiguration.CommandModifiers;
        if (mods == KeyModifiers.None)
            mods = KeyModifiers.Control;

        if ((e.KeyModifiers & mods) == mods)
        {
            _pointerManipulatingBoth = true;
            _absolutePosition = position;
            _isDrag = true;
            ShowToolTips();
            return;
        }

        var normalizedPosition = position * Math.Abs(Maximum - Minimum) / DragWidth;
        double upperValueDiff = Math.Abs(RangeEnd - normalizedPosition);
        double lowerValueDiff = Math.Abs(RangeStart - normalizedPosition);

        if (upperValueDiff < lowerValueDiff)
        {
            RangeEnd = normalizedPosition;
            _pointerManipulatingMax = true;
            HandleThumbDragStarted(_maxThumb);
        }
        else
        {
            RangeStart = normalizedPosition;
            _pointerManipulatingMin = true;
            HandleThumbDragStarted(_minThumb);
        }

        SyncThumbs();
    }

    private void UpdateToolTipTexts()
    {
        var format = ToolTipStringFormat;
        if (_minToolTipText != null)
        {
            _minToolTipText.Text = RangeStart.ToString(format);
        }
        if (_maxToolTipText != null)
        {
            _maxToolTipText.Text = RangeEnd.ToString(format);
        }
    }

    private void VerifyValues()
    {
        if (Minimum > Maximum)
        {
            Minimum = Maximum;
            Maximum = Maximum;
        }

        if (Minimum.Equals(Maximum))
        {
            Maximum += Epsilon;
        }

        if (!_maxSet)
        {
            RangeEnd = Maximum;
        }

        if (!_minSet)
        {
            RangeStart = Minimum;
        }

        if (RangeStart < Minimum)
        {
            RangeStart = Minimum;
        }

        if (RangeEnd < Minimum)
        {
            RangeEnd = Minimum;
        }

        if (RangeStart > Maximum)
        {
            RangeStart = Maximum;
        }

        if (RangeEnd > Maximum)
        {
            RangeEnd = Maximum;
        }

        if (RangeEnd < RangeStart)
        {
            RangeStart = RangeEnd;
        }
    }

    private void RangeMinToStepFrequency()
    {
        RangeStart = MoveToStepFrequency(RangeStart);
    }

    private void RangeMaxToStepFrequency()
    {
        RangeEnd = MoveToStepFrequency(RangeEnd);
    }

    private double MoveToStepFrequency(double rangeValue)
    {
        double newValue = Minimum + (((int)Math.Round((rangeValue - Minimum) / StepFrequency)) * StepFrequency);

        if (newValue < Minimum)
        {
            return Minimum;
        }
        else if (newValue > Maximum || Maximum - newValue < StepFrequency)
        {
            return Maximum;
        }
        else
        {
            return newValue;
        }
    }

    private void SyncThumbs()
    {
        if (_containerCanvas == null || _minThumb == null  || _maxThumb == null) { return; }

        var relativeLeft = ((RangeStart - Minimum) / (Maximum - Minimum)) * DragWidth;
        var relativeRight = ((RangeEnd - Minimum) / (Maximum - Minimum)) * DragWidth;

        Canvas.SetLeft(_minThumb, relativeLeft);
        Canvas.SetLeft(_maxThumb, relativeRight);

        if (_isDraggingStart)
        {
            _absolutePosition += (relativeLeft - _absolutePosition);
        }
        else if (_isDraggingEnd)
        {
            _absolutePosition += (relativeRight - _absolutePosition);
        }

        var y = _containerCanvas.Bounds.Height / 2 - _minThumb.Bounds.Height / 2;
        Canvas.SetTop(_minThumb, y);
        Canvas.SetTop(_maxThumb, y);

        SyncActiveRectangle();
    }

    private void SyncActiveRectangle()
    {
        if (_containerCanvas == null || _minThumb == null || _maxThumb == null|| _activeRectangle == null) { return; }

        var relativeLeft = Canvas.GetLeft(_minThumb);
        Canvas.SetLeft(_activeRectangle, relativeLeft);
        Canvas.SetTop(_activeRectangle, (_containerCanvas.Bounds.Height - _activeRectangle.Bounds.Height) / 2);
        _activeRectangle.Width = Math.Max(0, Canvas.GetLeft(_maxThumb) - Canvas.GetLeft(_minThumb));
    }

    private void ContainerCanvasSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        SyncThumbs();
    }

    private void ShowToolTips()
    {
        if (_minPopup == null || _maxPopup == null)
            return;

        if (!ShowValueToolTip)
            return;

        _minPopup.PlacementTarget = _minThumb;
        _minPopup.Placement = PlacementMode.Top;
        _minPopup.VerticalOffset = -12;
        _maxPopup.PlacementTarget = _maxThumb;
        _maxPopup.Placement = PlacementMode.Top;
        _maxPopup.VerticalOffset = -12;
        UpdateToolTipTexts();
        _minPopup.IsOpen = true;
        _maxPopup.IsOpen = true;
    }

    private void RestartCloseToolTipTimer()
    {
        _closeToolTipTimer.Stop();
        _closeToolTipTimer.Start();
    }

    private void ClosePopup(object? sender, EventArgs e)
    {
        if (_minPopup == null || _maxPopup == null || _isDrag)
            return;

        _minPopup.IsOpen = false;
        _maxPopup.IsOpen = false;
        _closeToolTipTimer.Stop();
    }
}
