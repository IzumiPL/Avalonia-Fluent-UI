using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using AvaloniaFluentUI.Controls;
using AvaloniaFluentUI.Windowing;

namespace MSFNavTest;

public partial class MainWindow : FluentWindow 
{
    public MainWindow()
    {
        InitializeComponent();
        TitleBarIsVisible = false;
        SplashScreen = new MainWindowSplashScreen(() => TitleBarIsVisible = true);

        Application.Current.Resources["NavigationViewContentGridCornerRadius"] = new CornerRadius(0);

        NavigationView.PropertyChanged += (_, e) =>
        {
            if (e.Property == NavigationView.IsPaneOpenProperty)
            {
                Console.WriteLine(NavigationView.DisplayMode);
                if (NavigationView.DisplayMode == NavigationViewDisplayMode.Minimal)
                {
                    TitleBarMargin = new Thickness(76, 0, 0, 0);
                }
                else if (NavigationView.DisplayMode == NavigationViewDisplayMode.Compact)
                {
                    TitleBarMargin = new Thickness(50, 0, 0, 0);
                }
                else
                {
                    TitleBarMargin = new Thickness(NavigationView.IsPaneOpen ? NavigationView.OpenPaneLength + 12 : 50, 0, 0, 0);
                }
            }

            if (e.Property == NavigationView.DisplayModeProperty)
            {
                if (NavigationView.DisplayMode == NavigationViewDisplayMode.Minimal)
                {
                    TitleBarMargin = new Thickness(76, 0, 0, 0);
                    SearchTextBox.IsVisible = false;
                }
                else
                {
                    if (!SearchTextBox.IsVisible)
                    {
                        SearchTextBox.IsVisible = true;
                    }
                }
            }
        };
    }

    private void OnPointerRelease(object? sender, PointerReleasedEventArgs e)
    {
        e.Handled = true; 
    }
}


internal class MainWindowSplashScreen(Action action) : IApplicationSplashScreen
{
    private Action Action { get; } = action;
    
    public object SplashScreenContent => new TextBlock
    {
        Text = "Application Splash Screen",
        FontSize = 32,
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
    };
    
    public async Task RunTasks(CancellationToken cancellationToken)
    {
        await Task.Delay(2000, cancellationToken);
        Action.Invoke();

        Task.Run(async () =>
        {
            int i = 0;
            while (++i < 10)
            {
                Console.WriteLine($"I: {i}");
                await Task.Delay(500);
            }
        });
    }

    public int MinimumShowTime => 2000;
}
