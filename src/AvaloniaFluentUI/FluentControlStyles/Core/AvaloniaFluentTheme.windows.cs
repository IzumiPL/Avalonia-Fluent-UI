using System;
using Avalonia.Controls;
using Avalonia.Logging;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Styling;
using AvaloniaFluentUI.Controls.Interop;
using AvaloniaFluentUI.Controls.Interop.WinRT;

namespace AvaloniaFluentUI.Styling;

public partial class AvaloniaFluentTheme
{
    private ThemeVariant ResolveWindowsSystemSettings(IPlatformSettings platformSettings)
    {
        ThemeVariant theme = null;
        if (PreferSystemTheme)
        {
            theme = GetThemeFromIPlatformSettings(platformSettings);
        }

        if (CustomAccentColor != null)
        {
            LoadCustomAccentColor();
        }
        else if (PreferUserAccentColor)
        {
            try
            {
                TryLoadWindowsAccentColor();
            }
            catch
            {
                Logger.TryGet(LogEventLevel.Information, "FluentAvaloniaTheme")?
                        .Log("FluentAvaloniaTheme", "Unable to create instance of ComObject IUISettings");
                LoadDefaultAccentColor();
            }            
        }
        else
        {
            LoadDefaultAccentColor();
        }

        return theme;
    }

    private void TryLoadWindowsAccentColor()
    {
        try
        {
            var settings3 = WinRTInterop.CreateInstance<IUISettings3>("Windows.UI.ViewManagement.UISettings");

            UpdateAccentColors((Color)settings3.GetColorValue(UIColorType.Accent),
                (Color)settings3.GetColorValue(UIColorType.AccentLight1),
                (Color)settings3.GetColorValue(UIColorType.AccentLight2),
                (Color)settings3.GetColorValue(UIColorType.AccentLight3),
                (Color)settings3.GetColorValue(UIColorType.AccentDark1),
                (Color)settings3.GetColorValue(UIColorType.AccentDark2),
                (Color)settings3.GetColorValue(UIColorType.AccentDark3));
        }
        catch
        {
            Logger.TryGet(LogEventLevel.Information, "FluentAvaloniaTheme")?
                .Log("FluentAvaloniaTheme", "Loading system accent color failed, using fallback (SlateBlue)");

            // We don't know where it failed, so override all
            LoadDefaultAccentColor();
        }
    }

    /// <summary>
    /// On Windows, forces a specific <see cref="Window"/> to the current theme
    /// </summary>
    /// <param name="window">The window to force</param>
    /// <param name="theme">The theme to use, or null to use the current RequestedTheme</param>
    /// <exception cref="ArgumentNullException">If window is null</exception>
    public void ForceWin32WindowToTheme(Window window, ThemeVariant theme = null)
    {
        if (window == null)
            throw new ArgumentNullException(nameof(window));

        if (!OperatingSystem.IsWindows())
            return;

        try
        {
            Win32Interop.ApplyTheme(window.TryGetPlatformHandle().Handle, theme == ThemeVariant.Dark);
        }
        catch
        {
            Logger.TryGet(LogEventLevel.Information, "FluentAvaloniaTheme")?
                        .Log("FluentAvaloniaTheme", "Unable to set window to theme.");
        }
    }
}
