using System;
using System.Diagnostics;
using Avalonia.Controls;

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

    public static async void OpenUrl(Uri uri, TopLevel topLevel)
    {
        try
        {
            await TopLevel.GetTopLevel(topLevel).Launcher.LaunchUriAsync(uri);
        }
        catch (Exception e) { }
    }
}
