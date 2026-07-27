using System;
using System.IO;
using System.Text;
using System.Text.Json;
using Avalonia;
using Avalonia.Styling;
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
            Directory.CreateDirectory(ConfigDir);
            var json = JsonSerializer.Serialize(config, ConfigJsonContext.Default.AppConfig);
            File.WriteAllText(AppConfigPath, json, Encoding.UTF8);
        }
        catch (Exception e) { }
    }

    public static AppConfig? LoadConfig()
    {
        Directory.CreateDirectory(ConfigDir);

        if (!File.Exists(AppConfigPath))
        {
            var config = new AppConfig
            {
                Theme = "Default",
                IsCustomAccentColor = false,
                IsWindowEffectEnabled = true,
                IsEnabledBackgroundImage = false,
                WindowEffect = "Null",
                Language = "zh-CN",
                BackgroundImagePath = null
            };

            return config;
        }

        try
        {
            string file = File.ReadAllText(AppConfigPath);
            var loaded = JsonSerializer.Deserialize(file, ConfigJsonContext.Default.AppConfig);

            return loaded;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public static bool IsDarkTheme() => Application.Current?.RequestedThemeVariant == ThemeVariant.Dark;
}
