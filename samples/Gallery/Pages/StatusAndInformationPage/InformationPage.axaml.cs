using System.Collections.Generic;
using Avalonia.Interactivity;
using Avalonia.Media;
using Gallery.Controls;

namespace Gallery.Pages;

public partial class InformationPage : ViewBase 
{
    public InformationPage() : base("Information")
    {
        InitializeComponent();

        CodeCards = new Dictionary<string, CodeCard>()
        {
            { "InfoBadge", InfoBadgeCard } 
        };
    }

    private void OnSelectedColorChanged(object? sender, Color c)
    {
        CtTag.Foreground = new SolidColorBrush(c);
    }

    private void OnClicked(object? sender, RoutedEventArgs e)
    {
        CtTag.ContextMenu?.Close();
    }
}

