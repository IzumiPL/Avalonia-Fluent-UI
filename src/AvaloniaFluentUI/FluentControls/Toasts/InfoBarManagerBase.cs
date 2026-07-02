using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;

namespace AvaloniaFluentUI.Controls;

/// <summary>
/// Base class for info-bar managers.
/// Handles host binding, positioning, stacking, show / remove lifecycle,
/// and auto-resize when the host changes size.
///
/// Position enums must use the standard 6-position layout:
/// TopLeft=0, Top=1, TopRight=2, BottomLeft=3, Bottom=4, BottomRight=5.
/// </summary>
public abstract class InfoBarManagerBase<TControl> : IInfoBarManager
    where TControl : InfoBarBase
{
    private InfoBarHost _host = null!;
    private readonly Dictionary<int, List<TControl>> _items = new();
    private readonly DispatcherTimer _updateLayoutTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };

    protected double HostMaxWidth => _host.Bounds.Width / 2.2;

    public double Spacing { get; set; } = 0;

    public double Margin { get; set; } = 12;

    public void SetHost(InfoBarHost host)
    {
        _host = host;
        _host.SizeChanged -= OnHostSizeChanged;
        _host.SizeChanged += OnHostSizeChanged;
    }

    public InfoBarManagerBase()
    {
        _updateLayoutTimer.Tick += (_, _) =>
        {
            AdjustedSize();
            UpdateAllInfoBarPosition();
        };
    }

    private void OnHostSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        // 只有在尺寸改变结束后才更新布局,尺寸
        _updateLayoutTimer.Stop();
        _updateLayoutTimer.Start();
    }

    public void UpdateAllInfoBarPosition()
    {
        for (int i = 0; i <= 5; i++)
        {
            UpdateInfoBarPosition(i);
        }
    }

    public void AdjustedSize()
    {
        foreach (var items in _items.Values)
        {
            foreach (var toast in items)
            {
                toast.MaxWidth = HostMaxWidth;
            }
        }
    }

    public void New(TControl toast)
    {
        Add(toast);
    }

    private void OnToastClosed(object? sender, EventArgs e)
    {
        if (sender is not TControl toast) return;
        toast.Closed -= OnToastClosed;
        Remove(toast);
    }

    public void Add(TControl toast)
    {
        GetInfoBars(toast.PositionValue).Add(toast);
        _host.Children.Add(toast);
        toast.Closed += OnToastClosed;

        Show(toast);
    }

    public void Show(TControl toast)
    {
        Dispatcher.UIThread.Post(() =>
        {
            var size = _host.Bounds.Size;
            var (sx, sy) = SlideStartPosition(toast, size);
            var (ex, ey) = SlideEndPosition(toast, size);
        
            toast.Run(sx, sy, ex, ey);
        }, DispatcherPriority.Render);
    }

    public async void Remove(TControl toast)
    {
        var value = toast.PositionValue;
        var size = _host.Bounds.Size;
        var (sx, sy) = SlideStartPosition(toast, size);

        GetInfoBars(value).Remove(toast);
        await toast.CloseAsync(sx, sy);
        _host.Children.Remove(toast);

        UpdateInfoBarPosition(value);
    }

    public (double x, double y) SlideEndPosition(TControl toast, Size hostSize)
    {
        int pos = toast.PositionValue;
        double width = toast.Bounds.Width;
        double height = toast.Bounds.Height;

        double x;
        double y;

        switch (pos)
        {
            // TopLeft
            case 0: 
                x = Margin;
                y = Margin;
                break;
            // Top
            case 1: 
                x = (hostSize.Width - width) / 2;
                y = Margin;
                break;
            // TopRight
            case 2:
                x = hostSize.Width - width - Margin;
                y = Margin;
                break;
            // BottomLeft
            case 3:
                x = Margin;
                y = hostSize.Height - height - Margin;
                break;
            // Bottom
            case 4:
                x = (hostSize.Width - width) / 2;
                y = hostSize.Height - height - Margin;
                break;
            // BottomRight
            case 5:
                x = hostSize.Width - width - Margin;
                y = hostSize.Height - height - Margin;
                break;
            default:
                x = Margin;
                y = Margin;
                break;
        }

        var toastList = GetInfoBars(pos);
        int index = toastList.IndexOf(toast);
        bool isTop = pos <= 2; // TopLeft, Top, TopRight

        for (int i = 0; i < index; i++)
        {
            var bar = toastList[i];
            y += isTop ? (bar.Bounds.Height + Spacing) : -(bar.Bounds.Height + Spacing);
        }

        return (x, y);
    }

    public (double x, double y) SlideStartPosition(TControl toast, Size hostSize)
    {
        var (x, y) = SlideEndPosition(toast, hostSize);

        return toast.PositionValue switch
        {
            1 => (x, y - toast.Bounds.Height - Spacing),    // Top
            0 => (-toast.Bounds.Width, y),                  // TopLeft
            2 => (hostSize.Width, y),                       // TopRight
            4 => (x, y + toast.Bounds.Height + Spacing),    // Bottom
            3 => (-toast.Bounds.Width, y),                  // BottomLeft
            5 => (hostSize.Width, y),                       // BottomRight
            _ => (x, y),
        };
    }

    public void UpdateInfoBarPosition(int value)
    {
        var toastList = GetInfoBars(value);
        if (toastList.Count == 0) return;

        var size = _host.Bounds.Size;
        foreach (var toast in toastList)
        {
            var (x, y) = SlideEndPosition(toast, size);
            toast.UpdatePosition(x, y);
        }
    }

    public List<TControl> GetInfoBars(int value)
    {
        if (_items.TryGetValue(value, out var items))
        {
            return items;
        }

        var list = new List<TControl>();
        _items[value] = list;
        return list;
    }
}
