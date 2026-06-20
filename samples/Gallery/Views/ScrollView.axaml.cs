using System;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using AvaloniaFluentUI.Locale;
using Gallery.Controls;

namespace Gallery.Views;

public partial class ScrollView : ViewBase
{
    private Bitmap? _b1;
    private Bitmap? _b2;
    
    public ScrollView()
    {
        InitializeComponent();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        
        _b1 =  Bitmap.DecodeToHeight(AssetLoader.Open(new Uri("avares://Gallery/Assets/Images/0.jpg")), 1024);
        _b2 = Bitmap.DecodeToHeight(AssetLoader.Open(new Uri("avares://Gallery/Assets/Images/mc.jpg")), 600);

        VImage.Source = _b1;
        HImage.Source = _b2;
        VHImage.Source = _b2;
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);

        VImage.Source = null;
        HImage.Source = null;
        VHImage.Source = null;
        
        _b1?.Dispose();
        _b2?.Dispose();
        _b1 = null;
        _b2 = null;
    }
}
