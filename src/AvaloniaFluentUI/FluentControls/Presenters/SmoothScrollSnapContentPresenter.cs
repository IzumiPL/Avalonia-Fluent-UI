using System;
using Avalonia.Input;

namespace AvaloniaFluentUI.Controls;

/// <summary>
/// 适用于 TimePicker, DatePicker的平滑滚动吸附
/// </summary>
public class SmoothScrollSnapContentPresenter : SmoothScrollContentPresenter
{
    /// <summary>
    /// 每行滚动的高度
    /// </summary>
    // TODO: 使用反射获取ItemHeight在Aot模式下不可用, 暂时用固定值解决
    public double ItemHeight { get; set; } = 40;

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        if (ItemHeight <= 0)
        {
            base.OnPointerWheelChanged(e);
            return;
        }

        if (_cts == null)
        {
            _targetOffset = Offset;
        }

        bool horizontal = e.KeyModifiers.HasFlag(KeyModifiers.Alt);
        double current = horizontal ? _targetOffset.X : _targetOffset.Y;

        current = Math.Round(current / ItemHeight) * ItemHeight;

        double max = horizontal ? Math.Max(0, Extent.Width - Viewport.Width) : Math.Max(0, Extent.Height - Viewport.Height);

        double snappedMax = Math.Floor(max / ItemHeight) * ItemHeight;
        
        double epsilon = 1e-6;
        double index = current / ItemHeight;
        double next = e.Delta.Y < 0 ? Math.Floor(index + epsilon) + 1 : Math.Ceiling(index - epsilon) - 1;

        double target = Math.Clamp(next * ItemHeight + 1.0, 0, snappedMax);

        _targetOffset = horizontal ? _targetOffset.WithX(target) : _targetOffset.WithY(target);
        
        _ = ScrollToAsync(_targetOffset);
        e.Handled = true;
    }
}
