using System;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AvaloniaFluentUI.Windowing;

public partial class FluentWindow
{
    private MenuFlyout? _systemMenu;
    private const int ResizeBorderThickness = 8;

    [Flags]
    private enum ResizeDirection
    {
        None = 0,
        Left = 1,
        Right = 2,
        Top = 4,
        Bottom = 8,
        TopLeft = Top | Left,
        TopRight = Top | Right,
        BottomLeft = Bottom | Left,
        BottomRight = Bottom | Right
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void InitializeLinuxPlatform()
    {
        IsLinux = true;
        BuildSystemMenu();
    }

    private void BuildSystemMenu()
    {
        _systemMenu = new MenuFlyout();

        var restoreItem = new MenuItem { Header = "还原(_R)" };
        restoreItem.Click += (_, _) => WindowState = WindowState.Normal;

        var moveItem = new MenuItem { Header = "移动(_M)" };
        moveItem.IsEnabled = false;

        var sizeItem = new MenuItem { Header = "大小(_S)" };
        sizeItem.IsEnabled = false;

        var minItem = new MenuItem { Header = "最小化(_N)" };
        minItem.Click += (_, _) => WindowState = WindowState.Minimized;

        var maxItem = new MenuItem { Header = "最大化(_X)" };
        maxItem.Click += (_, _) => WindowState = WindowState.Maximized;

        var closeItem = new MenuItem { Header = "关闭(_C)" };
        closeItem.Click += (_, _) => Close();
        closeItem.InputGesture = new KeyGesture(Key.F4, KeyModifiers.Alt);

        _systemMenu.Items.Add(restoreItem);
        _systemMenu.Items.Add(moveItem);
        _systemMenu.Items.Add(sizeItem);
        _systemMenu.Items.Add(new Separator());
        _systemMenu.Items.Add(minItem);
        _systemMenu.Items.Add(maxItem);
        _systemMenu.Items.Add(new Separator());
        _systemMenu.Items.Add(closeItem);
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if (!IsLinux) { return; }

        if (_windowBorder != null)
        {
            _windowBorder.PointerPressed += OnResizePointerPressed;
            _windowBorder.PointerMoved += OnResizePointerMoved;
        }

        if (_defaultTitleBar != null && _systemMenu != null)
        {
            _defaultTitleBar.ContextFlyout = _systemMenu;
        }
    }

    private ResizeDirection GetResizeDirection(Point pos)
    {
        var bounds = ClientSize;
        var direction = ResizeDirection.None;

        if (pos.X < ResizeBorderThickness)
            direction |= ResizeDirection.Left;
        else if (pos.X > bounds.Width - ResizeBorderThickness)
            direction |= ResizeDirection.Right;

        if (pos.Y < ResizeBorderThickness)
            direction |= ResizeDirection.Top;
        else if (pos.Y > bounds.Height - ResizeBorderThickness)
            direction |= ResizeDirection.Bottom;

        return direction;
    }

    private WindowEdge ToWindowEdge(ResizeDirection direction)
    {
        return direction switch
        {
            ResizeDirection.Left => WindowEdge.West,
            ResizeDirection.Right => WindowEdge.East,
            ResizeDirection.Top => WindowEdge.North,
            ResizeDirection.Bottom => WindowEdge.South,

            ResizeDirection.TopLeft => WindowEdge.NorthWest,
            ResizeDirection.TopRight => WindowEdge.NorthEast,

            ResizeDirection.BottomLeft => WindowEdge.SouthWest,
            ResizeDirection.BottomRight => WindowEdge.SouthEast,

            _ => WindowEdge.East
        };
    }

    private void UpdateCursor(ResizeDirection direction)
    {
        Cursor = direction switch
        {
            ResizeDirection.TopLeft => new Cursor(StandardCursorType.TopLeftCorner),
            ResizeDirection.TopRight => new Cursor(StandardCursorType.TopRightCorner),
            ResizeDirection.BottomLeft => new Cursor(StandardCursorType.BottomLeftCorner),
            ResizeDirection.BottomRight => new Cursor(StandardCursorType.BottomRightCorner),
            ResizeDirection.Left => new Cursor(StandardCursorType.LeftSide),
            ResizeDirection.Right => new Cursor(StandardCursorType.RightSide),
            ResizeDirection.Top => new Cursor(StandardCursorType.TopSide),
            ResizeDirection.Bottom => new Cursor(StandardCursorType.BottomSide),
            _ => new Cursor(StandardCursorType.Arrow)
        };
    }

    private void OnResizePointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (WindowState != WindowState.Normal)
            return;

        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            return;

        var pos = e.GetPosition(this);
        var direction = GetResizeDirection(pos);

        if (direction == ResizeDirection.None)
            return;

        BeginResizeDrag(ToWindowEdge(direction), e);

        e.Handled = true;
    }

    private void OnResizePointerMoved(object? sender, PointerEventArgs e)
    {
        var pos = e.GetPosition(this);
        UpdateCursor(GetResizeDirection(pos));
    }
}
