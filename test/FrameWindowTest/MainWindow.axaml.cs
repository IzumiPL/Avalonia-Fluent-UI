using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using AvaloniaFluentUI.Styling;
using FrameWindowTest.Controls;

namespace FrameWindowTest;

public partial class MainWindow : FrameWindow 
{
    public MainWindow()
    {
        InitializeComponent();

        // Background = Brush.Parse("#A1FFFFFF");

        // EnabledAcrylicBlue(true);
        EnabledMica(true);
        AvaloniaFluentTheme.Instance.ThemeChanged += OnThemeChanged;
    }

    private void OnThemeChanged(object? sender, ThemeVariant e)
    {
        // Background = Brush.Parse(e == ThemeVariant.Dark ? "#A1000000" : "#A1FFFFFF");

        // EnabledAcrylicBlue(true);
        EnabledMica(true);
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        SetCurrentValue(IconProperty, new WindowIcon(new Bitmap(AssetLoader.Open(new Uri("avares://FrameWindowTest/Assets/app.ico")))));
    }

    private void OnToggleTheme(object? sender, RoutedEventArgs e)
    {
        AvaloniaFluentTheme.Instance.ToggleTheme();
    }
}
