using System;
using System.Diagnostics;

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
}
