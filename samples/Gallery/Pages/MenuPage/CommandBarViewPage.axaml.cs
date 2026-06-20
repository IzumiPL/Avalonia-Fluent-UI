using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using AvaloniaFluentUI.Controls;
using Gallery.Controls;
using Gallery.Extensions;

namespace Gallery.Pages;

public partial class CommandBarViewPage : ViewBase
{
    private Bitmap? _bitmap = null;
    
    public CommandBarViewPage() : base("CommandBar")
    {
        InitializeComponent();
        Image.PointerReleased += OnImagePointerReleased;
        
        CodeCards = new Dictionary<string, CodeCard>()
        {
            {"CommandBar", CommandBarCard},
            {"CommandBarFlyout", CommandBarFlyoutCard}
        };
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (_bitmap == null)
        {
            _bitmap = Bitmap.DecodeToWidth(AssetLoader.Open(new Uri("avares://Gallery/Assets/Images/0.jpg")), 800);
            Image.Source = _bitmap;
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        Image.Source = null;
        _bitmap?.Dispose();
        _bitmap = null;
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
