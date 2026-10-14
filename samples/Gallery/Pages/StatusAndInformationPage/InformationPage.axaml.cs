using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class InformationPage : ViewBase 
{
    public override Uri? Uri => new Uri("https://github.com/IzumiPL/Avalonia-Fluent-UI/blob/master/samples/Gallery/Pages/StatusAndInformationPage/InformationPage.axaml"); 
    
    public InformationPage() : base("Information")
    {
        InitializeComponent();

        CodeCards = new Dictionary<string, CodeCard>()
        {
            { "InfoBadge", InfoBadgeCard } 
        };
    }

    private void OnSelectedColorChanged(object? sender,  ColorChangedEventArgs e)
    {
        CtTag.Foreground = new SolidColorBrush(e.NewColor);
    }

    private void OnClicked(object? sender, RoutedEventArgs e)
    {
        CtTag.ContextMenu?.Close();
    }
}

