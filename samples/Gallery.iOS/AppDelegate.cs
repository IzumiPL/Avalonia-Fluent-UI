using Foundation;
using Avalonia;
using Avalonia.iOS;
using Gallery;

namespace Gallery.iOS;

[Register("AppDelegate")]
public partial class AppDelegate : AvaloniaAppDelegate<App>
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
        => builder.UseiOS();
}
