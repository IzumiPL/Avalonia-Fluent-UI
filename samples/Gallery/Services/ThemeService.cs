using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Avalonia.Styling;
using AvaloniaFluentUI.Styling;
using Gallery.Models;

namespace Gallery.Services;

public class ThemeService
{
    private static string ConfigDir => Path.Combine(AppContext.BaseDirectory, "Config");
    // private static string ConfigDir => Path.Combine("/home/interval", "Config");

    private static string AppConfigPath => Path.Combine(ConfigDir, "config.json");

        // private static string ConfigDir => $@"C:\AppConfig";
        // private static string AppConfigPath => @"C:\AppConfig\config.json";
    
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
            Console.WriteLine("BaseDirectory: " + AppContext.BaseDirectory);
            Console.WriteLine("CurrentDirectory: " + Environment.CurrentDirectory);
            Console.WriteLine("FullPath: " + Path.GetFullPath(AppConfigPath));
        
            Directory.CreateDirectory(ConfigDir); 
            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            
            // await File.WriteAllTextAsync(AppConfigPath, json, Encoding.UTF8);
            File.WriteAllText(AppConfigPath, json, Encoding.UTF8);
        }
        catch (Exception e)
        {
            Console.WriteLine("Write Failed");
            Console.WriteLine(e);
            throw;
        }
    }

    public static async Task<AppConfig?> LoadConfig()
    {
        Console.WriteLine("BaseDirectory: " + AppContext.BaseDirectory);
        Console.WriteLine("CurrentDirectory: " + Environment.CurrentDirectory);
        Console.WriteLine("FullPath: " + Path.GetFullPath(AppConfigPath));
        
        AppConfig? config;
        
        Directory.CreateDirectory(ConfigDir);
        if (!File.Exists(AppConfigPath))
        {
            config = new AppConfig
            {
                Theme = "Default",
                AccentColor =  "#FFFF1493",
                IsWindowEffectEnabled = true,
                IsEnabledBackgroundImage = false
            };

            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });

            // File.Create(AppConfigPath);
            await File.WriteAllTextAsync(AppConfigPath, json, Encoding.UTF8);
            // File.WriteAllText(AppConfigPath, json, Encoding.UTF8);
            
            Application.Current?.RequestedThemeVariant = ThemeVariant.Default;
            FluentTheme?.CustomAccentColor = Colors.DeepPink;
        }
        else
        {
            string file = await File.ReadAllTextAsync(AppConfigPath);
            config = JsonSerializer.Deserialize<AppConfig>(file);
            if (config != null)
            {
                Application.Current?.RequestedThemeVariant = config.Theme switch
                {
                    "Light" => ThemeVariant.Light,
                    "Dark" => ThemeVariant.Dark,
                    _ => ThemeVariant.Default
                };
                FluentTheme?.CustomAccentColor = Color.Parse(config.AccentColor);
            }
        }

        return config;
    }

    public static bool IsDarkTheme() => Application.Current?.RequestedThemeVariant == ThemeVariant.Dark;
}
