using System;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaFluentUI.Controls;
using AvaloniaFluentUI.Locale;
using Gallery.Pages;
using Gallery.Services;
using Gallery.ViewModels;
using Gallery.Views;

namespace Gallery;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void InitializeCulture()
    {
        var localeDir = System.IO.Path.Combine(System.AppContext.BaseDirectory, "Assets", "Locale");
        if (System.IO.Directory.Exists(localeDir))
        {
            LocalizationService.Instance.LoadResxDirectory(localeDir);
        }
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Console.WriteLine(LocalizationService.Instance.GetString("SV_ThemeColorDescription"));
        var config = ConfigService.LoadConfig();
        Console.WriteLine($"Loaded Language: {config?.Language}");
        LocalizationService.Instance.SetCulture(config?.Language);
        Console.WriteLine($"Set Language: {LocalizationService.Instance.CurrentLanguage}");
        InitializeCulture();

        Console.WriteLine(LocalizationService.Instance.GetString("SV_ThemeColorDescription"));
        
        try
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Avoid duplicate validations from both Avalonia and the CommunityToolkit.
                // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(config)
                };
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
            {
                singleView.MainView = new MainView
                {
                    DataContext = new MainWindowViewModel(config)
                };
            }
            else
            {
                Console.Error.WriteLine($"Unhandled ApplicationLifetime type: {ApplicationLifetime?.GetType()}");
            }

            Frame.RegisterPage<FramePage1>();
            Frame.RegisterPage<FramePage2>();
            Frame.RegisterPage<FramePage3>();
            Frame.RegisterPage<FramePage4>();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"FATAL: App initialization failed: {ex}");
        }
        
        base.OnFrameworkInitializationCompleted();
    }

    private void OnClicked(object? sender, EventArgs e)
    {
        // Environment.Exit(0);
        if (Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow?.Close();
        }
    }
}
