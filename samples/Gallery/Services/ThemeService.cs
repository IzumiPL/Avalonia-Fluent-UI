using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Avalonia;
using Avalonia.Media;
using Avalonia.Styling;
using AvaloniaFluentUI.Locale;
using AvaloniaFluentUI.Styling;
using Gallery.Models;

namespace Gallery.Services;

public class ThemeService
{
    private static string ConfigDir => Path.Combine(AppContext.BaseDirectory, "Config");
    private static string AppConfigPath => Path.Combine(ConfigDir, "config.json");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        TypeInfoResolver = ConfigJsonContext.Default
    };

    public static event Action<ThemeVariant>? ThemeChanged;
    public static FluentAvaloniaTheme? FluentTheme { get; private set; }

    static ThemeService()
    {
        var app = Application.Current;
        if (app != null)
        {
            app.PropertyChanged += (_, e) =>
            {
                if (e.Property.Name == nameof(Application.ActualThemeVariant))
                {
                    ThemeChanged?.Invoke(app.ActualThemeVariant);
                }
            };
            FluentTheme = app.Styles.OfType<FluentAvaloniaTheme>().FirstOrDefault();
        }
    }

    public static void SetTheme(ThemeVariant variant)
    {
        Application.Current?.RequestedThemeVariant = variant;
    }

    public static void SetAccentColor(Color color)
    {
        FluentTheme?.CustomAccentColor = color;
    }

    public static void ToggleTheme()
    {
        var theme = IsDarkTheme() ? ThemeVariant.Light : ThemeVariant.Dark;
        Application.Current?.RequestedThemeVariant = theme;
    }

    public static void SaveConfig(AppConfig config)
    {
        try
        {
#if DEBUG
            Debug.WriteLine("BaseDirectory: " + AppContext.BaseDirectory);
            Debug.WriteLine("CurrentDirectory: " + Environment.CurrentDirectory);
            Debug.WriteLine("FullPath: " + Path.GetFullPath(AppConfigPath));
#endif

            Directory.CreateDirectory(ConfigDir);
            var json = JsonSerializer.Serialize(config, ConfigJsonContext.Default.AppConfig);
            File.WriteAllText(AppConfigPath, json, Encoding.UTF8);
        }
        catch (Exception e)
        {
#if DEBUG
            Debug.WriteLine("Write Failed");
            Debug.WriteLine(e);
#endif
        }
    }

    public static AppConfig? LoadConfig()
    {
#if DEBUG
        Debug.WriteLine("BaseDirectory: " + AppContext.BaseDirectory);
        Debug.WriteLine("CurrentDirectory: " + Environment.CurrentDirectory);
        Debug.WriteLine("FullPath: " + Path.GetFullPath(AppConfigPath));
#endif

        Directory.CreateDirectory(ConfigDir);

        if (!File.Exists(AppConfigPath))
        {
            var config = new AppConfig
            {
                Theme = "Default",
                AccentColor = "#FFFF1493",
                IsWindowEffectEnabled = true,
                IsEnabledBackgroundImage = false,
                Language = "zh-CN"
            };

            var json = JsonSerializer.Serialize(config, ConfigJsonContext.Default.AppConfig);
            File.WriteAllText(AppConfigPath, json, Encoding.UTF8);

            Application.Current?.RequestedThemeVariant = ThemeVariant.Default;
            FluentTheme?.CustomAccentColor = Colors.DeepSkyBlue;
            
            return config;
        }

        string file = File.ReadAllText(AppConfigPath);
        var loaded = JsonSerializer.Deserialize(file, ConfigJsonContext.Default.AppConfig);

        if (loaded != null)
        {
            Application.Current?.RequestedThemeVariant = loaded.Theme switch
            {
                "Light" => ThemeVariant.Light,
                "Dark" => ThemeVariant.Dark,
                _ => ThemeVariant.Default
            };
            FluentTheme?.CustomAccentColor = Color.Parse(loaded.AccentColor);
        }

        return loaded;
    }

    public static bool IsDarkTheme() => Application.Current?.RequestedThemeVariant == ThemeVariant.Dark;
}
