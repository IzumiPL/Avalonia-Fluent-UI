using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;
using AvaloniaFluentUI.Media.Animation;

namespace Gallery.Views;

public partial class IconsView : UserControl
{
    public IconsView()
    {
        InitializeComponent();
    }
    
    private void OnToggleThemeClicked(object? sender, RoutedEventArgs e)
    {
        var app = Application.Current;
        if (app != null)
        {
            var theme = app.RequestedThemeVariant == ThemeVariant.Light ? ThemeVariant.Dark : ThemeVariant.Light;
            app.RequestedThemeVariant = theme;
        }
    }
}
