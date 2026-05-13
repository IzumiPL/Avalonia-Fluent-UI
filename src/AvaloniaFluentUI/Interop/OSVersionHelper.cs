using System;

namespace AvaloniaFluentUI.Interop;

/// <summary>
/// Helper alowed to detect OS - keeping the IsWindows11 helper method here, all other
/// helpers pre2.5 are now replaced with direct calls to "OperatingSystem" class
/// </summary>
internal static class OSVersionHelper
{
    /// <summary>
    /// Return if current OS is Windows 11
    /// </summary>
    public static bool IsWindows11() =>
        OperatingSystem.IsWindowsVersionAtLeast(10, 0, 22000);
}
