using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using AvaloniaFluentUI.Controls;
using AvaloniaFluentUI.Icons;
using AvaloniaFluentUI.Locale;
using AvaloniaFluentUI.Styling;
using AvaloniaFluentUI.Windowing;
using CommunityToolkit.Mvvm.Messaging;
using Gallery.Messages;
using Gallery.Messages.MainWindowMessages;
using Gallery.Models;
using Gallery.Services;
using Gallery.ViewModels;

namespace Gallery.Views;

public class MainWindowSplashScreen : IApplicationSplashScreen
{
    public string AppName => "Avalonia Fluent UI Gallery";
    public IImage AppIcon => new Bitmap(AssetLoader.Open(new Uri("avares://Gallery/Assets/app.ico")));
    public object SplashScreenContent => null;
    public Task RunTasks(CancellationToken cancellationToken)
    {
        return Task.Delay(600, cancellationToken);
    }

    public int MinimumShowTime => 1500;
}

public partial class MainWindow : AppWindow
{
    public MainWindow()
    { 
        SplashScreen = new MainWindowSplashScreen();
        InitializeComponent();
        
        RegisterMessages();
        
        BackgroundImage.Source = Bitmap.DecodeToHeight(AssetLoader.Open(new Uri("avares://Gallery/Assets/Images/bg.jpg")), 1024);

        Loaded += OnLoaded;
        
        ToolTip.SetTip(PinButton, LocalizationService.Instance.GetString("Pin"));
        LocalizationService.Instance.PropertyChanged += (_, _) =>
        {
            if (PinButton.Tag!.ToString() == "isTopmost")
            {
                ToolTip.SetTip(PinButton, LocalizationService.Instance.GetString("UnPin"));
            }
            else
            {
                ToolTip.SetTip(PinButton, LocalizationService.Instance.GetString("Pin"));
            }
        };
    }

    private void RegisterMessages()
    {
        WeakReferenceMessenger.Default.Register<JumpToControlMessage>(this, OnJumpToControl);
        WeakReferenceMessenger.Default.Register<EnabledWindowEffectMessage>(this, OnEnabledWindowEffect);
        WeakReferenceMessenger.Default.Register<EnabledBackgroundImageMessage>(this, OnEnabledBackgroundImage);
    }

    private void OnEnabledBackgroundImage(object recipient, EnabledBackgroundImageMessage message)
    {
        BackgroundImage.IsVisible = message.IsVisible;
        if (message.IsVisible)
        {
            EnabledAcrylicBlue(false);
            EnabledMica(false);
        }
    }

    private void OnEnabledWindowEffect(object recipient, EnabledWindowEffectMessage message)
    {
        if (message.IsEnabled)
        {
            switch (message.type)
            {
                case "Mica":
                    EnabledMica(true);
                    break;
                case "Acrylic":
                    EnabledAcrylicBlue(true);
                    break;
            }
            return;
        }
        EnabledAcrylicBlue(false);
        EnabledMica(false);
    }

    private void OnJumpToControl(object recipient, JumpToControlMessage message)
    {
        foreach (var item in NavigationView.MenuItems)
        {
            if (item is NavigationViewItem nvi)
            {
                if (nvi.Tag!.ToString() == message.Page)
                {
                    NavigationView.SelectedItem = nvi;
                }
            }
        }
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            var svm = viewModel.SettingsViewModel;
            try
            {
                ConfigService.SaveConfig(new AppConfig
                {
                    AccentColor = svm.IsDefaultAccentColor ? FluentAvaloniaTheme.Instance.CurrentAccentColor.ToString() : svm.SelectedAccentColor.ToString(),
                    Theme = FluentAvaloniaTheme.Instance.CurrentTheme.ToString(),
                    IsWindowEffectEnabled = svm.IsEnabledWindowEffect,
                    WindowEffect = svm.CurrentEffect,
                    IsEnabledBackgroundImage = svm.IsEnabledBackgroundImage,
                    Language = svm.CurrentLanguage
                });
#if DEBUG
                Debug.WriteLine("Save Config Success");
#endif
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
#if DEBUG
        else Debug.WriteLine("Save Config Error");
#endif
        base.OnClosing(e);
    }

    private async void OnLoaded(object? sender, RoutedEventArgs e)
    {
        NavigationView.SettingsItem.Tag = "Settings";

        if (DataContext is MainWindowViewModel viewModel)
        {
            var svm = viewModel.SettingsViewModel;
            BackgroundImage.IsVisible = svm.IsEnabledBackgroundImage;
        }

        await PreloadViewsAsync();
    }

    private async Task PreloadViewsAsync()
    {
        await Task.Delay(499);

        var app = Application.Current;
        if (app == null) return;

        foreach (var template in app.DataTemplates)
        {
            if (template is ViewLocator locator)
            {
                await Dispatcher.UIThread.InvokeAsync(
                    () => locator.PreloadAllAsync(),
                    DispatcherPriority.Background);
                break;
            }
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (Topmost && change.Property == WindowStateProperty)
        {
            Topmost = false;
            Topmost = true;
        }
    }

    public void SetBackgroundImageIsVisible(bool visible)
    {
        BackgroundImage.IsVisible = visible;
    }

    private void OnToggleTopmost(object? sender, RoutedEventArgs e)
    {
        if (sender is ToolButton btn)
        {
            if (btn.Tag!.ToString() == "isTopmost")
            {
                btn.Tag = "noTopmost";
                btn.Content= FluentIcon.Pin;
                this.Topmost = false;
                ToolTip.SetTip(btn, LocalizationService.Instance.GetString("Pin"));
            }
            else
            {
                btn.Tag = "isTopmost";
                btn.Content = FluentIcon.Unpin;
                this.Topmost = true;
                ToolTip.SetTip(btn, LocalizationService.Instance.GetString("UnPin"));
            }
        }
    }
}
