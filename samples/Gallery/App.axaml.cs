using System;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using AvaloniaFluentUI.Styling;
using AvaloniaFluentUI.UI.Controls;
using Gallery.Pages;
using Gallery.ViewModels;
using Gallery.Views;

namespace Gallery;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        try
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Avoid duplicate validations from both Avalonia and the CommunityToolkit.
                // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
                DisableAvaloniaDataAnnotationValidation();
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel()
                };
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
            {
                singleView.MainView = new MainView
                {
                    DataContext = new MainWindowViewModel()
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

            // SetLanguage("zh-CN");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"FATAL: App initialization failed: {ex}");
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void SetLanguage(string language)
    {
        var culture = new CultureInfo(language);
        
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove) BindingPlugins.DataValidators.Remove(plugin);
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
