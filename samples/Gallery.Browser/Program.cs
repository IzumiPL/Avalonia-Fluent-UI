using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Gallery;

namespace Gallery.Browser;

internal sealed partial class Program
{
    private static async Task Main(string[] args)
    {
        try
        {
            await BuildAvaloniaApp().StartBrowserAppAsync("out");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"FATAL: Gallery.Browser startup failed: {ex}");
            throw;
        }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UseBrowser()
            .LogToTrace();
}
