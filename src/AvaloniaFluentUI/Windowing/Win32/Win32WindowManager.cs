using System;
using System.Collections.Generic;
using static AvaloniaFluentUI.Controls.Interop.Win32Interop;
using Avalonia.Controls;
using Avalonia;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Avalonia.Platform;
using AvaloniaFluentUI.Controls.Interop;
using AvaloniaFluentUI.Controls.Interop.Win32;

namespace AvaloniaFluentUI.Windowing;

internal unsafe class Win32WindowManager
{
    public Win32WindowManager(FluentWindow window)
    {
        _window = window;

        Hwnd = (HWND)_window.TryGetPlatformHandle().Handle;
        
        _oldWndProc = GetWindowLongPtrW(Hwnd, GWLP_WNDPROC);

        _appWindowRegistry.Add(Hwnd, this);

        // Apparently...nint and void* aren't blittable types to the mono-wasm compiler
        // so the function pointer here needs to use IntPtr
        _wndProc = (nint)(delegate* unmanaged<IntPtr, uint, IntPtr, IntPtr, IntPtr>)&WndProcStatic;

        SetWindowLongPtrW(Hwnd, GWLP_WNDPROC, _wndProc);

        var ps = Application.Current?.PlatformSettings;
        ps.ColorValuesChanged += OnPlatformColorValuesChanged;
        _window.Closed += WindowOnClosed;
    }

    public HWND Hwnd { get; }

    /// <summary>
    /// 启用 Windows 10 的 NC 渲染禁用(见 <see cref="DisableNonClientRendering"/>)
    /// </summary>
    internal void EnableNonClientRenderingWorkaround()
    {
        _disableNonClientRendering = true;
        DisableNonClientRendering();
    }

    /// <summary>
    /// 关闭 DWM 对窗口非客户区的渲染
    /// </summary>
    private void DisableNonClientRendering()
    {
        if (!_disableNonClientRendering)
        {
            return;
        }

        var policy = DWMNCRENDERINGPOLICY.DWMNCRP_DISABLED;
        _ = DwmSetWindowAttribute(Hwnd, DWMWINDOWATTRIBUTE.DWMWA_NCRENDERING_POLICY, &policy, sizeof(int));
    }

    /// <summary>
    /// 会导致 DWM 重建窗口框架的消息, 处理完这些消息后需要重新关闭 NC 渲染
    /// </summary>
    private static bool IsFrameRebuildMessage(uint msg) => msg switch
    {
        0x0005 => true, // WM_SIZE
        0x0006 => true, // WM_ACTIVATE
        0x0047 => true, // WM_WINDOWPOSCHANGED
        0x0086 => true, // WM_NCACTIVATE
        0x02E0 => true, // WM_DPICHANGED
        0x031A => true, // WM_THEMECHANGED
        0x031E => true, // WM_DWMCOMPOSITIONCHANGED
        0x001A => true, // WM_SETTINGCHANGE
        _ => false
    };

    private LRESULT WndProc(HWND hWnd, uint msg, WPARAM wParam, LPARAM lParam)
    {
        // DWM 会在重建窗口框架时重新打开非客户区渲染, 所以这些消息处理完后要再关一次,
        // 否则圆角缺口处会再次出现黑线.
        if (IsFrameRebuildMessage(msg))
        {
            DisableNonClientRendering();
        }

        switch (msg)
        {
            case WM_RBUTTONUP:
                HandleRBUTTONUP(lParam);
                break;

            case WM_SYSCOMMAND:
                // Enables ALT+SPACE to open the system menu
                if ((WPARAM)wParam == SC_KEYMENU)
                {
                    return DefWindowProcW((HWND)hWnd, msg, (WPARAM)wParam, lParam);
                }
                break;

            case WM_DESTROY:
                _appWindowRegistry.Remove(hWnd);
                break;
        }

        return CallWindowProcW(_oldWndProc, hWnd, msg, wParam, lParam);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private double GetScaling() =>
        _window.RenderScaling;

    private unsafe void HandleRBUTTONUP(LPARAM lParam)
    {
        var pt = PointFromLParam(lParam);

        if (_window.HitTestTitleBar(pt.ToPoint(GetScaling())))
        {
            var sysMenu = GetSystemMenu((HWND)Hwnd, false);
            bool isMax = _window.WindowState == WindowState.Maximized;
            bool isDialog = _window.ShowAsDialog;

            bool canMinimize = _window.CanMinimize && !isDialog;
            bool canMaximize = _window.CanMaximize && !isDialog;
            bool canSize = _window.CanResize && !isDialog;
            bool isNormalState = !isMax && !isDialog;

            var mii = new MENUITEMINFO
            {
                cbSize = (uint)sizeof(MENUITEMINFO),
                fMask = MIIM_STATE,
                fState = MFS_ENABLED
            };
            SetMenuItemInfo(sysMenu, SC_CLOSE, false, &mii);

            mii.fState = (uint)(canMinimize ? MFS_ENABLED : MFS_DISABLED);
            SetMenuItemInfo(sysMenu, SC_MINIMIZE, false, &mii);
            
            // Restore only enabled if maximized
            mii.fState = (uint)((isMax && !isDialog) ? MFS_ENABLED : MFS_DISABLED);
            SetMenuItemInfo(sysMenu, SC_RESTORE, false, &mii);

            mii.fState = (uint)(isNormalState ? MFS_ENABLED : MFS_DISABLED);
            SetMenuItemInfo(sysMenu, SC_MOVE, false, &mii);

            mii.fState = (uint)((isNormalState && canSize) ? MFS_ENABLED : MFS_DISABLED);
            SetMenuItemInfo(sysMenu, SC_SIZE, false, &mii);

            mii.fState = (uint)((isNormalState && canMaximize) ? MFS_ENABLED : MFS_DISABLED);
            SetMenuItemInfo(sysMenu, SC_MAXIMIZE, false, &mii);

            SetMenuDefaultItem(sysMenu, uint.MaxValue, 0);

            var scPt = _window.PointToScreen(pt.ToPoint(GetScaling()));

            var ret = TrackPopupMenu(sysMenu, TPM_RETURNCMD, scPt.X, scPt.Y, 0, (HWND)Hwnd, null);
            if (ret)
            {
                PostMessage((HWND)Hwnd, WM_SYSCOMMAND, (WPARAM)ret, 0);
            }
        }
    }

    private void OnPlatformColorValuesChanged(object? sender, PlatformColorValues e)
    {
        // We need to override Avalonia's default setting of this to always keep AppWindow
        // in dark mode, which matches what windows do on Win 10/11, regardless of the actual
        // app or system theme.
        Win32Interop.ApplyTheme(Hwnd, true);
        // ApplyTheme 内部会用 SWP_FRAMECHANGED 让 DWM 重建框架, 这会重置上面的禁用, 需要补一次.
        DisableNonClientRendering();
    }
    
    private void WindowOnClosed(object? sender, EventArgs e)
    {
        var ps = Application.Current?.PlatformSettings;
        ps.ColorValuesChanged -= OnPlatformColorValuesChanged;
        _window.Closed -= WindowOnClosed;
    }

    [UnmanagedCallersOnly]
    private static IntPtr WndProcStatic(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        if (_appWindowRegistry.TryGetValue((HWND)hwnd, out var wnd))
        {
            return wnd.WndProc((HWND)hwnd, msg, (WPARAM)wParam, (LPARAM)lParam);
        }

        return (IntPtr)0;
    }

    private static Dictionary<HWND, Win32WindowManager> _appWindowRegistry =
        new Dictionary<HWND, Win32WindowManager>();


    private readonly FluentWindow _window;

    private bool _disableNonClientRendering;

    private readonly nint _oldWndProc;
    private readonly nint _wndProc;
}
