using System;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace Test.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    
        // ShortcutKeyPanel.Keys = ["Ctrl", "Alt", "C"];
    }
    
    public static string GenerateHexColor()
    {
        var random = new Random();

        int r = random.Next(0, 256);
        int g = random.Next(0, 256);
        int b = random.Next(0, 256);

        return $"#{r:X2}{g:X2}{b:X2}";
    }

    private void OnChangedBg(object? sender, RoutedEventArgs e)
    {
        Avatar.Background = Brush.Parse(GenerateHexColor());
    }

    private void OnChangedFg(object? sender, RoutedEventArgs e)
    {
        Avatar.Foreground = Brush.Parse(GenerateHexColor());
    }
}

