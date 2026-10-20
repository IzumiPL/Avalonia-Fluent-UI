using Avalonia;
using Avalonia.Media;

namespace AvaloniaFluentUI.Fonts;

public static class Fonts
{
    public static AppBuilder WithFont(this AppBuilder builder)
    {
        builder.AfterSetup(_ =>
        {
            if (Application.Current is not { } app)
            {
                return;
            }

            app.Resources["DefaultFontFamily"] = new FontFamily("avares://AvaloniaFluentUI.Fonts/Assets/Fonts/MiSans.otf#MiSans");
        });
        
        return builder;
    }
}
