using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaFluentUI.Styling;
using Gallery.Helpers;

namespace Gallery.Pages;

public partial class PopupDrawerPage : UserControl 
{
    public Uri? Uri => new Uri("https://github.com/IzumiPL/Avalonia-Fluent-UI/blob/master/samples/Gallery/Pages/DialogAndPopupPage/PopupDrawerPage.axaml"); 
    
    public PopupDrawerPage()
    {
        InitializeComponent();
    }

    private void OnDocumentButtonClicked(object? sender, RoutedEventArgs e)
    {
        UrlHelpers.OpenUrl(new Uri("https://docs.mikuas.top/"), TopLevel.GetTopLevel(this));
    }

    private void OnSourceCodeButtonClicked(object? sender, RoutedEventArgs e)
    {
        if (Uri != null)
        {
            UrlHelpers.OpenUrl(Uri, TopLevel.GetTopLevel(this));
        }
    }

    private void OnToggleThemeClicked(object? sender, RoutedEventArgs e) => AvaloniaFluentTheme.Instance.ToggleTheme();

    private async void OnShowDialog(object? sender, RoutedEventArgs e)
    {
        Dialog.DataContext = this.DataContext;
        await Dialog.ShowAsync(TopLevel.GetTopLevel(this));
    }
}
