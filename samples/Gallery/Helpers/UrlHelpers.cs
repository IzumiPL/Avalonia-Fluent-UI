using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace Gallery.Helpers;

public class UrlHelpers
{
    public static void OpenUrl(string url)
    {
        try
        {
            if (String.IsNullOrWhiteSpace(url)) { return; }
            
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch (Exception e) { }
    }

    public static async void OpenUrl(Uri uri, Visual? visual)
    {
        try
        {
            var toplevel = TopLevel.GetTopLevel(visual);
            if (toplevel == null) { return; }
            
            await toplevel.Launcher.LaunchUriAsync(uri);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }
    }

    public static async void OpenUrl(Uri uri)
    {
        OpenUrl(uri, GetTopLevel());
    }

    public static TopLevel? GetTopLevel()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow;
        }
        else if (Application.Current?.ApplicationLifetime is ISingleViewApplicationLifetime single)
        {
            return TopLevel.GetTopLevel(single.MainView);
        }
        
        return null;
    }
}
