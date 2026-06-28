using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Dialogs;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using AvaloniaFluentUI.Controls;
using AvaloniaFluentUI.Icons;
using AvaloniaFluentUI.Windowing;
using AvaloniaFluentUI.Locale;
using AvaloniaFluentUI.Styling;

namespace Test.Views;

public partial class MainWindow : FluentWindow
{
    private MainWindowSplashScreen SplashScreen = new MainWindowSplashScreen();
    
    public MainWindow()
    {
        InitializeComponent();
        
        // SplashScreen = new MainWindowSplashScreen();
        // EnabledAcrylicBlue(true);
        
        AvaloniaFluentTheme.Instance.ThemeChanged += (_, theme) =>
        {
            Console.WriteLine($"Theme Changed: {theme}");
            Console.WriteLine(Background.ToString());
            
            // Background = Brush.Parse(FluentAvaloniaTheme.Instance.IsDarkTheme ? "#30161616" : "#30F3F3F3");  
        };

        AvaloniaFluentTheme.Instance.ThemeColorChanged += (_, color) =>
        {
            Console.WriteLine($"Theme Color Changed: {color}");
        };

        
    }

    private void OnClicked(object? sender, RoutedEventArgs e)
    {
        AvaloniaFluentTheme.Instance.CurrentAccentColor = GetRandomColor();
    }

    public Color GetRandomColor()
    {
        var random = new Random();
        return Color.Parse($"#{random.Next(256):X2}" +
                           $"{random.Next(256):X2}" +
                           $"{random.Next(256):X2}");
    }

    private void ToggleToDefaultColor(object? sender, RoutedEventArgs e)
    {
        AvaloniaFluentTheme.Instance.CustomAccentColor = null;
    }

    private void OnAcrylicClicked(object? sender, RoutedEventArgs e)
    {
        if (sender is CheckBox cb)
        {
            EnabledAcrylicBlue((bool)cb.IsChecked);
        }
    }

    private void OnCreateWindowClicked(object? sender, RoutedEventArgs e)
    {
        var window = new FluentWindow();
        window.Title = "Window";
        var b1 = new Border
        {
            Background = Brush.Parse("#f1f3f9"),
            BorderThickness = new Thickness(0, 0, 1, 0),
            BorderBrush = Brush.Parse("#10000000")
        };
        var b2 = new Border { Background = Brush.Parse("#f7f9fc") };
        window.Content = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("52, *"),
            Children =
            {
                b1,
                b2,
            }
        };
        Grid.SetColumn(b1, 0);
        Grid.SetColumn(b2, 1);
        window.SplashScreen = SplashScreen;
        window.Icon = this.Icon;
        window.TitleBarMargin = new Thickness(48, 0, 0, 0);
        
        b1.Child = new StackPanel
        {
            Spacing = 3,
            Margin =  new Thickness(0, 6, 0, 0),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Top,
            Children =
            {
                new ToolButton{ Classes = { "Transparent" }, Content = FluentIcon.Return },
                new ToolButton{ Classes = { "Transparent" }, Content = FluentIcon.Menu },
                new ToolButton{ Classes = { "Transparent" }, Content = FluentIcon.Ringer },
                new ToolButton{ Classes = { "Transparent" }, Content = FluentIcon.Sync },
            }
        };
        
        window.Show();
    }
}

class MainWindowSplashScreen : IApplicationSplashScreen
{
    public string AppName => "Test";
    public IImage AppIcon => null;
    public object SplashScreenContent => null; 
    public Task RunTasks(CancellationToken cancellationToken)
    {
        return Task.Delay(1000);
    }

    public int MinimumShowTime => 1000;
}
