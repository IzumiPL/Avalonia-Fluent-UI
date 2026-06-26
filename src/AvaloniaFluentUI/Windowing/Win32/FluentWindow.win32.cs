using System;
using System.Runtime.CompilerServices;
using AvaloniaFluentUI.Controls.Interop;

namespace AvaloniaFluentUI.Windowing;

public partial class FluentWindow
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void InitializeWindowPlatform()
    {
        Console.WriteLine("Is Window Platform");
        
        IsWindows = true;
        IsWindows11 = OSVersionHelper.IsWindows11();

        _win32Manager = new Win32WindowManager(this);

        Win32Interop.ApplyTheme(_win32Manager.Hwnd, true);
        PlatformFeatures = new Win32AppWindowFeatures(this);
    }

    private Win32WindowManager _win32Manager;
}
