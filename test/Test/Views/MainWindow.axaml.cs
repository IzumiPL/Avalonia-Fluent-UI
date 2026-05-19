using System.Collections.Generic;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Styling;
using AvaloniaFluentUI.UI.Controls;

namespace Test.Views;

public partial class MainWindow : Window
{
    private TextBlock _block = new TextBlock { Text = "你好", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center};
    private FlipView _page = new FlipView { DecodeToHeight = 800 };
    private int _ct = 0;
    
    public MainWindow()
    {
        InitializeComponent();
        _page.ImageSource = new List<string>
            {
                "avares://Test/Assets/1.jpg",
                "avares://Test/Assets/2.jpg",
                "avares://Test/Assets/3.jpg",
                "avares://Test/Assets/4.jpg",
                "avares://Test/Assets/5.jpg",
                "avares://Test/Assets/6.jpg",
                "avares://Test/Assets/7.jpg"
            };
        // Grid.Children.Add(_page);
        
        Application.Current?.RequestedThemeVariant = ThemeVariant.Light;
    }

    private void ChangeImage(object? sender, RoutedEventArgs e)
    {
        ++_ct;
        if (_ct % 2 != 0)
        {
            _page.ImageSource = new List<string>()
            {
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\0.webp",
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\1.webp",
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\2.webp",
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\3.webp",
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\4.webp",
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\5.webp",
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\6.webp",
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\7.webp",
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\8.webp",
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\9.webp",
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\10.webp",@"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\0.webp",
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\1.webp",
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\2.webp",
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\3.webp",
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\4.webp",
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\5.webp",
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\6.webp",
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\7.webp",
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\8.webp",
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\9.webp",
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\10.webp",@"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\0.webp",
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\1.webp",
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\2.webp",
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\3.webp",
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\4.webp",
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\5.webp",
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\6.webp",
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\7.webp",
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\8.webp",
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\9.webp",
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\10.webp",@"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\0.webp",
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\1.webp",
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\2.webp",
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\3.webp",
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\4.webp",
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\5.webp",
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\6.webp",
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\7.webp",
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\8.webp",
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\9.webp",
                @"C:\Users\Administrator\OneDrive\Pictures\Genshin Impact\Launcher\10.webp",
            };
        }
        else
        {
            _page.ImageSource = new List<string>
            {
                "avares://Test/Assets/1.jpg",
                "avares://Test/Assets/2.jpg",
                "avares://Test/Assets/3.jpg",
                "avares://Test/Assets/4.jpg",
                "avares://Test/Assets/5.jpg",
                "avares://Test/Assets/6.jpg",
                "avares://Test/Assets/7.jpg","avares://Test/Assets/1.jpg",
                "avares://Test/Assets/2.jpg",
                "avares://Test/Assets/3.jpg",
                "avares://Test/Assets/4.jpg",
                "avares://Test/Assets/5.jpg",
                "avares://Test/Assets/6.jpg",
                "avares://Test/Assets/7.jpg","avares://Test/Assets/1.jpg",
                "avares://Test/Assets/2.jpg",
                "avares://Test/Assets/3.jpg",
                "avares://Test/Assets/4.jpg",
                "avares://Test/Assets/5.jpg",
                "avares://Test/Assets/6.jpg",
                "avares://Test/Assets/7.jpg","avares://Test/Assets/1.jpg",
                "avares://Test/Assets/2.jpg",
                "avares://Test/Assets/3.jpg",
                "avares://Test/Assets/4.jpg",
                "avares://Test/Assets/5.jpg",
                "avares://Test/Assets/6.jpg",
                "avares://Test/Assets/7.jpg",
            };
        }
    }

    private void OnToggleTheme(object? sender, RoutedEventArgs e)
    {
        Application.Current?.RequestedThemeVariant = Application.Current?.RequestedThemeVariant == ThemeVariant.Light ?
            ThemeVariant.Dark :
            ThemeVariant.Light;
    }
}
