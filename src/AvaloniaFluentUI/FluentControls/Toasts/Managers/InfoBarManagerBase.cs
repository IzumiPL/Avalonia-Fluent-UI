using System;
using System.Collections.Generic;
using System.Linq;
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
public abstract class InfoBarManagerBase<TControl> : IInfoBarManager where TControl : InfoBarBase
{
    private InfoBarHost _host;
    private readonly Dictionary<InfoBarPosition, List<TControl>> _items = new();
    private readonly DispatcherTimer _updateLayoutTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };

    public double InfoBarMaxWidth => _host.Bounds.Width / 2.2;

    public double Spacing { get; set; } = 0;

    public double Margin { get; set; } = 12;

    public void SetHost(InfoBarHost host)
    {
        _host?.SizeChanged -= OnHostSizeChanged;
        _host = host;
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
        var positions = Enum.GetValues<InfoBarPosition>();
        foreach (var position in positions)
        {
            UpdateInfoBarPosition(position);
        }
    }

    public void AdjustedSize()
    {
        foreach (var items in _items.Values)
        {
            foreach (var bar in items)
            {
                bar.MaxWidth = InfoBarMaxWidth;
            }
        }
    }
    
    private void OnInfoBarClosed(object? sender, EventArgs e)
    {
        if (sender is not TControl bar) return;
        bar.Closed -= OnInfoBarClosed;
        CloseAsync(bar);
    }

    protected void Add(TControl bar)
    {
        GetInfoBars(bar.Position).Add(bar);
        _host.Children.Add(bar);
        bar.Closed += OnInfoBarClosed;

        Show(bar);
    }

    protected async void CloseAsync(TControl bar)
    {
        var position = bar.Position;
        var size = _host.Bounds.Size;
        var (sx, sy) = SlideStartPosition(bar, size);

        GetInfoBars(position).Remove(bar);
        await bar.CloseAsync(sx, sy);
        _host.Children.Remove(bar);

        UpdateInfoBarPosition(position);
    }

    public void CloseAll()
    {
        foreach (var position in Enum.GetValues<InfoBarPosition>())
        {
            CloseAll(position);
        }
    }
    
    public void CloseAll(InfoBarPosition position)
    {
        var bars = GetInfoBars(position);
        foreach (var bar in bars)
        {
            _host.Children.Remove(bar);
        }
        bars.Clear();
    }
    
    private void Show(TControl bar)
    {
        Dispatcher.UIThread.Post(() =>
        {
            var size = _host.Bounds.Size;
            var (sx, sy) = SlideStartPosition(bar, size);
            var (ex, ey) = SlideEndPosition(bar, size);
        
            bar.Run(sx, sy, ex, ey);
        }, DispatcherPriority.Render);
    }

    public (double x, double y) SlideEndPosition(TControl bar, Size hostSize)
    {
        var position = bar.Position;
        double width = bar.Bounds.Width;
        double height = bar.Bounds.Height;

        double x;
        double y;

        switch (position)
        {
            // TopLeft
            case InfoBarPosition.TopLeft: 
                x = Margin;
                y = Margin;
                break;
            // Top
            case InfoBarPosition.Top: 
                x = (hostSize.Width - width) / 2;
                y = Margin;
                break;
            // TopRight
            case InfoBarPosition.TopRight:
                x = hostSize.Width - width - Margin;
                y = Margin;
                break;
            // BottomLeft
            case InfoBarPosition.BottomLeft:
                x = Margin;
                y = hostSize.Height - height - Margin;
                break;
            // Bottom
            case InfoBarPosition.Bottom:
                x = (hostSize.Width - width) / 2;
                y = hostSize.Height - height - Margin;
                break;
            // BottomRight
            case InfoBarPosition.BottomRight:
                x = hostSize.Width - width - Margin;
                y = hostSize.Height - height - Margin;
                break;
            default:
                x = Margin;
                y = Margin;
                break;
        }

        var bars = GetInfoBars(position);
        int index = bars.IndexOf(bar);
        bool isTop = position is InfoBarPosition.Top or InfoBarPosition.TopLeft or InfoBarPosition.TopRight; // TopLeft, Top, TopRight

        for (int i = 0; i < index; i++)
        {
            var item = bars[i];
            y += isTop ? (item.Bounds.Height + Spacing) : -(item.Bounds.Height + Spacing);
        }

        return (x, y);
    }

    public (double x, double y) SlideStartPosition(TControl bar, Size hostSize)
    {
        var (x, y) = SlideEndPosition(bar, hostSize);
        var position =  bar.Position;

        return position switch
        {
            InfoBarPosition.Top => (x, y - bar.Bounds.Height - Spacing),          // Top
            InfoBarPosition.TopLeft => (-bar.Bounds.Width, y),                    // TopLeft
            InfoBarPosition.TopRight => (hostSize.Width, y),                      // TopRight
            InfoBarPosition.Bottom => (x, y + bar.Bounds.Height + Spacing),       // Bottom
            InfoBarPosition.BottomLeft => (-bar.Bounds.Width, y),                 // BottomLeft
            InfoBarPosition.BottomRight => (hostSize.Width, y),                   // BottomRight
            _ => (x, y),
        };
    }

    public void UpdateInfoBarPosition(InfoBarPosition position)
    {
        var bars = GetInfoBars(position);
        if (bars.Count == 0) return;

        var size = _host.Bounds.Size;
        foreach (var bar in bars)
        {
            var (x, y) = SlideEndPosition(bar, size);
            bar.UpdatePosition(x, y);
        }
    }

    public List<TControl> GetInfoBars(InfoBarPosition position)
    {
        if (_items.TryGetValue(position, out var items))
        {
            return items;
        }

        var list = new List<TControl>();
        _items[position] = list;
        return list;
    }
}
