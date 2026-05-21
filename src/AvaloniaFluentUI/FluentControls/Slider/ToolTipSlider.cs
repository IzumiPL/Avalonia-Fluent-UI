using System;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Threading;

namespace AvaloniaFluentUI.Controls;

[TemplatePart(Name = "PART_Thumb", Type = typeof(Thumb))]
[TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
[TemplatePart(Name = "PART_ToolTipText", Type = typeof(TextBlock))]
public class ToolTipSlider : Slider
{
    private Popup? _popup;
    private TextBlock? _toolTipText;
    private Thumb? _thumb;

    private bool _isDrag;
    private readonly DispatcherTimer _closeToolTipTimer = new DispatcherTimer();

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

        _thumb = e.NameScope.Find<Thumb>("PART_Thumb");
        _popup = e.NameScope.Find<Popup>("PART_Popup");
        _toolTipText = e.NameScope.Find<TextBlock>("PART_ToolTipText");

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
            _popup.VerticalOffset = -Bounds.Height / 2;
        }
        else
        {
            _popup.Placement = PlacementMode.Left;
            _popup.HorizontalOffset = -Bounds.Height / 2.5;
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
            _toolTipText.Text = Value.ToString("0");
        }
    }

    private void OnThumbDragCompleted(object? sender, VectorEventArgs e)
    {
        Console.WriteLine("DragCompleted");
        _isDrag = false;
    }

    private void OnThumbDragStarted(object? sender, VectorEventArgs e)
    {
        Console.WriteLine("DragStarted");
        _isDrag = true;
    }
}
