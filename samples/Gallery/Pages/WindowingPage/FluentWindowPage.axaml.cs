using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Layout;
using AvaloniaFluentUI.Windowing;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class FluentWindowPage : ViewBase 
{
    public override Uri? Uri => new Uri("https://github.com/IzumiPL/Avalonia-Fluent-UI/blob/master/samples/Gallery/Pages/ViewPage/FluentWindowPage.axaml"); 
    
    public FluentWindowPage() : base("Windowing")
    {
        InitializeComponent();

        CodeCards = new Dictionary<string, CodeCard>()
        {
            { "FluentWindow", FluentWindowCard},
        };
    }

    private void OnShowWindow(object? sender, RoutedEventArgs e)
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
            return;
        
        new FluentWindow
        {
            Title = TitleBox.Text,
            Width = (double)WidthNumericUpDown.Value,
            Height = (double)HeightNumericUpDown.Value,
            Position = new PixelPoint((int)XNumericUpDown.Value, (int)YNumericUpDown.Value)
        }.Show();
    }

    private void OnShowFullscreenWindow(object? sender, RoutedEventArgs e)
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
            return;
        
        var button = new Button { Content = "Close", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Width = 256 };
        var window = new FluentWindow { WindowState = WindowState.FullScreen, Content = button, MinButtonIsVisible = false, MaxButtonIsVisible = false, CloseButtonIsVisible = false, CanResize = false, CanMaximize = false, CanMinimize = false };
        button.Click += (_, _) => { window.Close(); };
        
        window.Show();
    }

    private void OnShowCustomWindow(object? sender, RoutedEventArgs e)
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
            return;
        
        var window = new FluentWindow
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            Icon = ((FluentWindow)desktop.MainWindow).Icon,
            Title = "Fluent Window",
            Width = 900,
            Height = 600,
            Topmost = AOTTS.IsChecked == true,
            CanMaximize = MATS.IsChecked == true,
            CanMinimize = MITS.IsChecked == true,
            CanResize = RTS.IsChecked == true,
            MaxButtonIsVisible = MABVTS.IsChecked == true,
            MinButtonIsVisible = MIBVTS.IsChecked == true,
            CloseButtonIsVisible = CBVTS.IsChecked == true,
        };
        window.TitleBarTemplateSettings.IconIsVisible = IVTS.IsChecked == true;
        window.TitleBarTemplateSettings.TitleIsVisible = TVTS.IsChecked == true;
        window.Show();
    }
}
