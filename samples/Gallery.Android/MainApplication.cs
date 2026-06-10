using System;
using Android.App;
using Android.Runtime;
using Avalonia;
using Avalonia.Android;

namespace Gallery.Android;

[Application]
public class MainApplication : AvaloniaAndroidApplication<App>
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
    {
    }

    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
#if DEBUG
            .WithDeveloperTools()
#endif
            .LogToTrace();
    }
}
