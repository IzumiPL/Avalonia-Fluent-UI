using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Threading;

namespace AvaloniaFluentUI.Controls;

[TemplatePart(Name = PART_THUMB, Type = typeof(Thumb))]
[TemplatePart(Name = PART_POPUP, Type = typeof(Popup))]
[TemplatePart(Name = PART_TOOL_TIP_TEXT, Type = typeof(TextBlock))]
public class ToolTipSlider : Slider
{
    public static readonly StyledProperty<string> FormatProperty =
        AvaloniaProperty.Register<ToolTipSlider, string>(nameof(Format), defaultValue: "0");

    public string Format
    {
        get => GetValue(FormatProperty);
        set => SetValue(FormatProperty, value);
    }
    
    private Popup? _popup;
    private TextBlock? _toolTipText;
    private Thumb? _thumb;

    private bool _isDrag;
    private readonly DispatcherTimer _closeToolTipTimer = new DispatcherTimer();
    
    private const string PART_THUMB = "PART_Thumb";
    private const string PART_POPUP = "PART_Popup";
    private const string PART_TOOL_TIP_TEXT = "PART_ToolTipText";

    public ToolTipSlider()
    {
        _closeToolTipTimer.Interval = TimeSpan.FromMilliseconds(200);
        _closeToolTipTimer.Tick += ClosePopup;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (_thumb != null)
        {
            _thumb.DragDelta -= OnDragDelta;
            _thumb.DragStarted -= OnThumbDragStarted;
            _thumb.DragCompleted -= OnThumbDragCompleted;
        }

        _thumb = e.NameScope.Find<Thumb>(PART_THUMB);
        _popup = e.NameScope.Find<Popup>(PART_POPUP);
        _toolTipText = e.NameScope.Find<TextBlock>(PART_TOOL_TIP_TEXT);

        if (_thumb != null)
        {
            _thumb.DragDelta += OnDragDelta;
            _thumb.DragStarted += OnThumbDragStarted;
            _thumb.DragCompleted += OnThumbDragCompleted;
        }
    }

    private void OnDragDelta(object? sender, VectorEventArgs e)
    {
        UpdateToolTipText();
        ShowPopup();
        _closeToolTipTimer.Stop();
        _closeToolTipTimer.Start();
    }

    private void ShowPopup()
    {
        if (_popup == null) return;

        UpdateToolTipText();

        if (Orientation == Orientation.Horizontal)
        {
            _popup.Placement = PlacementMode.Top;
            _popup.VerticalOffset = -12;
        }
        else
        {
            _popup.Placement = PlacementMode.Left;
            _popup.HorizontalOffset = -12;
        }
        _popup.IsOpen = true;
    }

    private void ClosePopup(object? sender, EventArgs e)
    {
        if (_popup == null || _isDrag) { return; }
        _popup.IsOpen = false;
        _closeToolTipTimer.Stop();
    }

    private void UpdateToolTipText()
    {
        if (_toolTipText != null)
        {
            _toolTipText.Text = Value.ToString(Format);
        }
    }

    private void OnThumbDragCompleted(object? sender, VectorEventArgs e) => _isDrag = false;

    private void OnThumbDragStarted(object? sender, VectorEventArgs e) => _isDrag = true;
}
