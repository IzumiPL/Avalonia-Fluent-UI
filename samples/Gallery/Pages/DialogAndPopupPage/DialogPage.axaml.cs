using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using AvaloniaFluentUI.Controls;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class DialogPage : ViewBase
{
    private ColorDialog ColorDialog { get; } = new ColorDialog { Color = Colors.DeepPink };

    public override Uri? Uri => new Uri("https://github.com/IzumiPL/Avalonia-Fluent-UI/blob/master/samples/Gallery/Pages/DialogAndPopupPage/DialogPage.axaml");

    public DialogPage() : base("Dialog")
    {
        InitializeComponent();
        
        CodeCards = new Dictionary<string, CodeCard>()
        {
            {"TaskDialog", ContentDialogCard},
            {"ContentDialog", ContentDialogCard},
        };
    }
    
    private async void OnShowContentDialog(object? sender, RoutedEventArgs e)
    {
        ContentDialog.FullSizeDesired = FullHeightCheckBox.IsChecked == true;
        await ContentDialog.ShowAsync();
    }

    private async void OnShowCustomDialog(object? sender, RoutedEventArgs e)
    {
        await CustomDialog.ShowAsync();
    }

    private async void OnShowColorDialog(object? sender, RoutedEventArgs e)
    {
        if (await ColorDialog.ShowAsync(TopLevel.GetTopLevel(this)) == ContentDialogResult.Primary)
        {
            FilledPushButton.Background = new SolidColorBrush(ColorDialog.Color);
        }
    }
}
