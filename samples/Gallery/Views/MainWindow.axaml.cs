using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using AvaloniaFluentUI.Controls;
using AvaloniaFluentUI.Controls.Windowing;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.Messaging;
using Gallery.Messages;
using Gallery.Models;
using Gallery.Services;
using Gallery.Themes;
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
    private IPageTransition? _currentPageTransition;

    private bool _isEnabledWindowEffect;
    private readonly MainWindowMessageHandler _messageHandler;

    public MainWindow()
    { 
        SplashScreen = new MainWindowSplashScreen();
        InitializeComponent();
        
        _messageHandler = new MainWindowMessageHandler(this); 
        RegisterAllMessage();
        
        BackgroundImage.Source = Bitmap.DecodeToHeight(AssetLoader.Open(new Uri("avares://Gallery/Assets/Images/bg.jpg")), 1024);

        SetPageTransition(null);

        // ExtendClientAreaToDecorationsHint = true;

        Loaded += OnLoaded;
        ThemeService.ThemeChanged += _ => { EnableWindowEffect(_isEnabledWindowEffect); };

        WeakReferenceMessenger.Default.Register<JumpToControlMessage>(this, OnJumpToControl);

        Dispatcher.UIThread.Post(async () =>
        {
            await Task.Delay(3000);
            // Background = Brushes.DeepPink;
        }, DispatcherPriority.Background);


        ToolTip.SetTip(PinButton, LocalizationService.Instance.GetString("Pin"));

        LocalizationService.Instance.PropertyChanged += (_, __) =>
        {
            if (PinButton.Tag.ToString() == "isTopmost")
            {
                ToolTip.SetTip(PinButton, LocalizationService.Instance.GetString("UnPin"));
            }
            else
            {
                ToolTip.SetTip(PinButton, LocalizationService.Instance.GetString("Pin"));
            }
        };
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
                ThemeService.SaveConfig(new AppConfig
                {
                    AccentColor = svm.IsDefaultAccentColor ? ThemeService.FluentTheme?.CustomAccentColor.ToString() : svm.SelectedAccentColor.ToString(),
                    Theme = Application.Current?.RequestedThemeVariant.ToString(),
                    IsWindowEffectEnabled = svm.IsEnabledWindowEffect,
                    IsEnabledBackgroundImage = svm.IsEnabledBackgroundImage,
                    Language = svm.CurrentLanguage
                });
                Console.WriteLine("Save Config Success");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
        else { Console.WriteLine("Save Config Error");}
        base.OnClosing(e);
    }

    private async void OnLoaded(object? sender, RoutedEventArgs e)
    {
        NavigationView.SettingsItem.Tag = "Settings";

        if (DataContext is MainWindowViewModel viewModel)
        {
            var svm = viewModel.SettingsViewModel;
            EnableWindowEffect(svm.IsEnabledWindowEffect);
            BackgroundImage.IsVisible = svm.IsEnabledBackgroundImage;
        }

        // PlatformFeatures?.SetTaskBarProgressBarState(TaskBarProgressBarState.Normal); 
        // PlatformFeatures?.SetTaskBarProgressBarValue(50, 100);
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;
        // ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.PreferSystemChrome;
        
        await PreloadViewsAsync();
    }

    private async Task PreloadViewsAsync()
    {
        await Task.Delay(500);

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

    private void OnViewModelChanged(string page, string name)
    {
        foreach (var item in NavigationView.MenuItems)
        {
            if (item is NavigationViewItem nvi)
            {
                if ((string)nvi.Tag! == page) NavigationView.SelectedItem = nvi;

                WeakReferenceMessenger.Default.Send(new JumpToControlMessage(page, name));
            }
        }
    }

    private void RegisterAllMessage() => _messageHandler.RegisterAllMessage();

    private void InitControls()
    {
    }

    public void SetPageTransition(IPageTransition? pageTransition)
    {
        _currentPageTransition = pageTransition;
    }

    public IPageTransition? CurrentPageTransition => _currentPageTransition;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        InitControls();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (Topmost && change.Property == WindowStateProperty)
        {
            Topmost = false;
            Topmost = true;

            // if (change.GetNewValue<WindowState>() == WindowState.Maximized)
            // {
            //     NavigationView.Margin = new Thickness(12, 55, 8, 6);
            // }
            // else
            // {
            //     NavigationView.Margin = new Thickness(6, 55, 0, 0);
            // }
        }
    }

    public void SetBackgroundImageIsVisible(bool visible)
    {
        BackgroundImage.IsVisible = visible;
    }

    public void EnableWindowEffect(bool enable = true)
    {
        if (enable)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (Environment.OSVersion.Version.Build >= 22000)
                {
                    Background = Brushes.Transparent;
                    TransparencyLevelHint = [WindowTransparencyLevel.Mica];
                }
                else
                {
                    Background = Brush.Parse(ThemeService.IsDarkTheme() ? "#A1000000" : "#C1FFFFFF");
                    TransparencyLevelHint = [WindowTransparencyLevel.AcrylicBlur];
                }
            }
        }
        else
        {
#if DEBUG
            Debug.WriteLine("Apply Default Background Style");
#endif
            Background = Brush.Parse(ThemeService.IsDarkTheme() ? "#202020" : "#F0F4F9");
            TransparencyLevelHint = [WindowTransparencyLevel.None];
        }

        _isEnabledWindowEffect = enable;
    }

    private void OnClicked(object? sender, RoutedEventArgs e)
    {
        LocalizationService.Instance.SetCulture("en-US");
    }

    private void OnToggleTopmost(object? sender, RoutedEventArgs e)
    {
        if (sender is ToolButton btn)
        {
            if (btn.Tag.ToString() == "isTopmost")
            {
                btn.Tag = "noTopmost";
                btn.IconData = Geometry.Parse(FluentIcon.Pin);
                this.Topmost = false;
                ToolTip.SetTip(btn, LocalizationService.Instance.GetString("Pin"));
            }
            else
            {
                btn.Tag = "isTopmost";
                btn.IconData = Geometry.Parse(FluentIcon.Unpin);
                this.Topmost = true;
                ToolTip.SetTip(btn, LocalizationService.Instance.GetString("UnPin"));
            }
        }
    }
}
