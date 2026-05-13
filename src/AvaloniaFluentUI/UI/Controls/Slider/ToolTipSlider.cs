using System;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Threading;

namespace AvaloniaFluentUI.UI.Controls;

[TemplatePart(Name = "Thumb", Type = typeof(Thumb))]
[TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
[TemplatePart(Name = "PART_ToolTipText", Type = typeof(TextBlock))]
public class ToolTipSlider : Slider
{
    private Popup? _popup;
    private TextBlock? _toolTipText;
    private Thumb? _thumb;
    
    private readonly DispatcherTimer _timer = new DispatcherTimer();

    public ToolTipSlider()
    {
        _timer.Interval = TimeSpan.FromMilliseconds(300);
        _timer.Tick += ClosePopup;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (_thumb != null)
        {
            _thumb.DragDelta -= OnDragDelta;
        }
        
        
        _thumb = e.NameScope.Find<Thumb>("Thumb");
        _popup = e.NameScope.Find<Popup>("PART_Popup");
        _toolTipText = e.NameScope.Find<TextBlock>("PART_ToolTipText");

        if (_thumb != null)
        {
            _thumb.DragDelta += OnDragDelta;
        }
    }

    private void OnDragDelta(object? sender, VectorEventArgs e)
    {
        UpdateToolTipText();
        ShowPopup();
        _timer.Stop();
        _timer.Start();
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
    
    private void ClosePopup(object? sender,  EventArgs e)
    {
        if (_popup == null) return;
        _popup.IsOpen = false;
    }

    private void UpdateToolTipText()
    {
        if (_toolTipText != null)
        {
            _toolTipText.Text = Value.ToString("0");
        }
    }
}
