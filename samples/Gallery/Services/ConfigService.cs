using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using Avalonia;
using Avalonia.Media;
using Avalonia.Styling;
using AvaloniaFluentUI.Styling;
using Gallery.Models;

namespace Gallery.Services;

public class ConfigService
{
    private static string ConfigDir => Path.Combine(AppContext.BaseDirectory, "Config");
    private static string AppConfigPath => Path.Combine(ConfigDir, "config.json");

    static ConfigService()
    {
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
                AccentColor = "#00BFFF",
                IsWindowEffectEnabled = true,
                IsEnabledBackgroundImage = false,
                WindowEffect = "Mica",
                Language = "zh-CN"
            };

            var json = JsonSerializer.Serialize(config, ConfigJsonContext.Default.AppConfig);
            File.WriteAllText(AppConfigPath, json, Encoding.UTF8);

            Application.Current?.RequestedThemeVariant = ThemeVariant.Default;

            Console.WriteLine("Config File Not Exists, Return Of Create");
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
            FluentAvaloniaTheme.Instance.CurrentAccentColor = Color.Parse(loaded.AccentColor);
        }

        Console.WriteLine("Config File Loaded");
        return loaded;
    }

    public static bool IsDarkTheme() => Application.Current?.RequestedThemeVariant == ThemeVariant.Dark;
}
