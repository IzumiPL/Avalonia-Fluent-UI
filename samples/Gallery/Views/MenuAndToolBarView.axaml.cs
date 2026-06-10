using System;
using System.Collections.Generic;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using AvaloniaFluentUI.Controls;
using AvaloniaFluentUI.Locale;
using CommunityToolkit.Mvvm.Messaging;
using Gallery.Controls;
using Gallery.Messages;

namespace Gallery.Views;

public partial class MenuAndToolBarView : ViewBase 
{
    public MenuAndToolBarView() : base("MenuAndToolBar")
    {
        InitializeComponent();
        
        Image.Source = Bitmap.DecodeToWidth(AssetLoader.Open(new Uri("avares://Gallery/Assets/Images/0.jpg")), 512);
        Image.PointerReleased += OnImagePointerReleased;

        CodeCards = new Dictionary<string, CodeCard>()
        {
            {"Menu", MenuCard},
            {"MenuBar", MenuBarCard},
            {"CommandBar", CommandBarCard},
            {"CommandBarFlyout", CommandBarFlyoutCard}
        };
    }

    private void OnImagePointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        var flyout = Resources["CommandBarFlyout"] as CommandBarFlyout;
        if (flyout != null)
        {
            flyout.ShowAt(Image);
        }
    }
}
